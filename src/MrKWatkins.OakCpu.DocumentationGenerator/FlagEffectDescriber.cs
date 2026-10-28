using System.Text.RegularExpressions;
using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Describes the effect an instruction has on each flag in prose. Instructions set their flags in one of two ways: with
/// <c>flags()</c> and an expression per flag, as the Z80's do, or by writing the flags register directly, as the 6502's do with
/// <c>P = P &amp; mask</c> to reset flags and <c>P = P | bit</c>, usually under an <c>if</c>, to set them. The prose for the
/// CPU's own functions and temporaries comes from its definitions.
/// </summary>
public sealed partial class FlagEffectDescriber
{
    [GeneratedRegex(@"\{([^}]*)\}")]
    private static partial Regex PlaceholderRegex();

    private readonly CpuModel model;
    private readonly IReadOnlyDictionary<string, string> temporaryNames;
    private readonly Configuration configuration;
    private readonly Instruction instruction;
    private readonly IReadOnlyDictionary<string, string> placeholders;
    private readonly IReadOnlyList<FlagsCallSite> callSites;
    private readonly IReadOnlyDictionary<string, FlagState> registerStates;
    private readonly IReadOnlySet<string> assigned;
    private readonly List<KeyValuePair<string, string>> fallbacks = [];
    private Flag? describing;
    private List<string>? collectingFallbacks;
    private IReadOnlyDictionary<string, Expression?> temporaries = new Dictionary<string, Expression?>();

    /// <param name="model">The CPU's model.</param>
    /// <param name="instruction">The instruction.</param>
    /// <param name="placeholders">
    /// The placeholders for the instruction's operands, keyed by the operand, e.g. <c>B</c> = <c>{R0}</c>, so that the
    /// descriptions refer to placeholders rather than the operands of one opcode.
    /// </param>
    public FlagEffectDescriber(CpuModel model, Instruction instruction, IReadOnlyDictionary<string, string> placeholders)
    {
        this.model = model;
        configuration = model.Configuration;
        temporaryNames = model.Cpu.Temporaries;
        this.instruction = instruction;
        this.placeholders = placeholders;

        callSites = FindFlagsCallSites(instruction.Steps);

        assigned = ReachableSteps(instruction)
            .SelectMany(step => step.Statements)
            .SelectMany(statement => statement.TraverseDepthFirst())
            .OfType<Assignment>()
            .Where(assignment => assignment.Target is not TemporaryVariableAccess)
            .Select(assignment => assignment.Target.Name)
            .ToHashSet(StringComparer.Ordinal);

        registerStates = WalkSteps(instruction.Steps, 0, new PathState(), new HashSet<string>(StringComparer.Ordinal)).Flags;
    }

    /// <summary>
    /// The expressions that could not be described and so were described by their expression text, keyed by the name of the
    /// flag being described. Empty once the describer understands every expression the instruction uses.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, string>> Fallbacks => fallbacks;

    /// <summary>
    /// Describes the effect of the instruction on each of the CPU's flags, most significant first.
    /// </summary>
    [Pure]
    public IReadOnlyList<FlagEffect> Describe() =>
        configuration.Flags.Values
            .OrderByDescending(flag => flag.Index)
            .Select(flag =>
                registerStates.TryGetValue(flag.Name, out var state)
                    ? DescribeState(flag, state)
                    : instruction.Flags.TryGetValue(flag.Name, out var expression)
                        ? DescribeAtCallSites(flag, expression)
                        : FlagEffect.Unaffected(flag))
            .ToList();

    /// <summary>
    /// The steps of the instruction and of every sequence it can move to.
    /// </summary>
    [Pure]
    // ReSharper disable once ParameterHidesMember
    private IEnumerable<Step> ReachableSteps(Instruction instruction)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<IReadOnlyList<Step>>();
        queue.Enqueue(instruction.Steps);
        while (queue.TryDequeue(out var steps))
        {
            foreach (var step in steps)
            {
                yield return step;

                foreach (var sequence in step.Statements.SelectMany(statement => statement.TraverseDepthFirst()).OfType<SequenceAccess>())
                {
                    if (visited.Add(sequence.SequenceName))
                    {
                        queue.Enqueue(model.GetSequence(sequence.SequenceName).Steps);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Finds each call to <c>flags()</c> in the instruction's steps along with the temporaries in scope at the call, i.e. those
    /// assigned exactly once on the path to it. An instruction that can finish early, such as a repeating block instruction,
    /// calls <c>flags()</c> on each path, and the paths can set the flags differently.
    /// </summary>
    [Pure]
    private static IReadOnlyList<FlagsCallSite> FindFlagsCallSites(IReadOnlyList<Step> steps)
    {
        var callSites = new List<FlagsCallSite>();
        var definitions = new Dictionary<string, Expression?>(StringComparer.Ordinal);
        foreach (var step in steps)
        {
            FindFlagsCallSites(step.Statements, definitions, false, callSites);
        }

        return callSites;
    }

    private static void FindFlagsCallSites(IReadOnlyList<Statement> statements, Dictionary<string, Expression?> definitions, bool completes, List<FlagsCallSite> callSites)
    {
        completes = completes || statements.Any(statement => statement is CallStatement { Call.Function: var function } && function == PreDefinedFunction.InstructionComplete);

        foreach (var statement in statements)
        {
            switch (statement)
            {
                case Assignment { Target: TemporaryVariableAccess temporary } assignment:
                    definitions[temporary.Name] = assignment.Value;
                    break;

                case CallStatement { Call.Function: var function } when function == PreDefinedFunction.Flags:
                    callSites.Add(new FlagsCallSite(new Dictionary<string, Expression?>(definitions, StringComparer.Ordinal), completes));
                    break;

                case IfStatement ifStatement:
                    var ifDefinitions = new Dictionary<string, Expression?>(definitions, StringComparer.Ordinal);
                    FindFlagsCallSites(ifStatement.IfStatements, ifDefinitions, completes, callSites);
                    var elseDefinitions = new Dictionary<string, Expression?>(definitions, StringComparer.Ordinal);
                    FindFlagsCallSites(ifStatement.ElseStatements, elseDefinitions, completes, callSites);
                    RemoveDifferingTemporaries(definitions, ifDefinitions, elseDefinitions);
                    break;
            }
        }
    }

    /// <summary>
    /// A temporary assigned in a branch has a value that depends on the condition after the if, so cannot be described by
    /// either assignment.
    /// </summary>
    private static void RemoveDifferingTemporaries(Dictionary<string, Expression?> definitions, IReadOnlyDictionary<string, Expression?> ifDefinitions, IReadOnlyDictionary<string, Expression?> elseDefinitions)
    {
        foreach (var name in ifDefinitions.Keys.Concat(elseDefinitions.Keys).Distinct(StringComparer.Ordinal).ToList())
        {
            var before = definitions.GetValueOrDefault(name);
            if (!ReferenceEquals(ifDefinitions.GetValueOrDefault(name), before) || !ReferenceEquals(elseDefinitions.GetValueOrDefault(name), before))
            {
                definitions.Remove(name);
            }
        }
    }

    /// <summary>
    /// Walks the steps of a sequence from one of its steps onwards, tracking what each write to the flags register leaves each
    /// flag as. A step can move to a named sequence to carry on in, or complete the instruction.
    /// </summary>
    private PathState WalkSteps(IReadOnlyList<Step> steps, int index, PathState state, IReadOnlySet<string> visited)
    {
        if (index >= steps.Count)
        {
            return state;
        }

        return WalkStatements(steps[index].Statements, 0, state, new Continuation(null, false, visited, next => WalkSteps(steps, index + 1, next, visited)));
    }

    private PathState WalkStatements(IReadOnlyList<Statement> statements, int index, PathState state, Continuation continuation)
    {
        for (; index < statements.Count; index++)
        {
            switch (statements[index])
            {
                case Assignment { Target: TemporaryVariableAccess temporary } assignment:
                    // A temporary defined from itself, e.g. $high = $high + 0x10, no longer has a definition that can be described.
                    var selfReferential = assignment.Value.TraverseDepthFirst().OfType<TemporaryVariableAccess>().Any(access => access.Name == temporary.Name);
                    state.Temporaries[temporary.Name] = selfReferential ? null : assignment.Value;
                    break;

                case Assignment { Target: RegisterAccess { Register.Flags: true } } assignment:
                    temporaries = state.Temporaries;
                    foreach (var flag in configuration.Flags.Values)
                    {
                        if (BitOf(assignment.Value, flag.Index) is { } bit)
                        {
                            state.Flags[flag.Name] = bit;
                        }
                    }
                    break;

                case CallStatement { Call.Function: var function } when function == PreDefinedFunction.InstructionComplete:
                    continuation = continuation with { Completes = true };
                    break;

                case CallStatement { Call: { Function: var function, Arguments: [SequenceAccess sequence] } } when function == PreDefinedFunction.MoveToSequence:
                    continuation = continuation with { MoveToSequence = sequence.SequenceName };
                    break;

                case IfStatement ifStatement:
                    // The rest of the statements run after either branch, so walk them as part of each.
                    var rest = statements.Skip(index + 1).ToList();
                    temporaries = state.Temporaries;
                    var condition = ConditionOf(ifStatement.Condition);
                    var whenTrue = WalkStatements([.. ifStatement.IfStatements, .. rest], 0, state.Clone(), continuation);
                    var whenFalse = WalkStatements([.. ifStatement.ElseStatements, .. rest], 0, state.Clone(), continuation);
                    return Merge(condition, whenTrue, whenFalse);
            }
        }

        if (continuation.Completes)
        {
            return state;
        }

        if (continuation.MoveToSequence is { } name)
        {
            if (continuation.Visited.Contains(name))
            {
                return state;
            }

            return WalkSteps(model.GetSequence(name).Steps, 0, state, new HashSet<string>(continuation.Visited, StringComparer.Ordinal) { name });
        }

        return continuation.Next(state);
    }

    /// <summary>
    /// Merges the states after the two branches of an if: a flag the branches leave the same is unchanged by the if, otherwise
    /// its value depends on the condition.
    /// </summary>
    [Pure]
    private static PathState Merge(Condition condition, PathState whenTrue, PathState whenFalse)
    {
        var merged = new PathState();
        foreach (var flag in whenTrue.Flags.Keys.Concat(whenFalse.Flags.Keys).Distinct(StringComparer.Ordinal))
        {
            var t = whenTrue.Flags.GetValueOrDefault(flag) ?? UnchangedState.Instance;
            var f = whenFalse.Flags.GetValueOrDefault(flag) ?? UnchangedState.Instance;
            merged.Flags[flag] = t == f ? t : new ConditionalState(condition, t, f);
        }

        foreach (var (name, definition) in whenTrue.Temporaries)
        {
            if (whenFalse.Temporaries.TryGetValue(name, out var other) && ReferenceEquals(definition, other))
            {
                merged.Temporaries[name] = definition;
            }
        }

        return merged;
    }

    /// <summary>
    /// What a write of an expression to the flags register leaves a bit of it as, or <c>null</c> if it leaves it unchanged.
    /// </summary>
    [Pure]
    private FlagState? BitOf(Expression expression, int bit)
    {
        switch (expression)
        {
            case Number number:
                return new ConstantState((number.Value >> bit & 1) == 1);

            case RegisterAccess { Register.Flags: true }:
                return null;

            case BinaryOperation { Operator: var @operator } binary when @operator == Operator.And:
                {
                    var left = BitOf(binary.Left, bit);
                    var right = BitOf(binary.Right, bit);
                    if (left is ConstantState { Value: false } || right is ConstantState { Value: false })
                    {
                        return new ConstantState(false);
                    }

                    if (left is ConstantState { Value: true })
                    {
                        return right;
                    }

                    if (right is ConstantState { Value: true })
                    {
                        return left;
                    }

                    return left == null && right == null ? null : Copied(expression, bit);
                }

            case BinaryOperation { Operator: var @operator } binary when @operator == Operator.Or:
                {
                    var left = BitOf(binary.Left, bit);
                    var right = BitOf(binary.Right, bit);
                    if (left is ConstantState { Value: true } || right is ConstantState { Value: true })
                    {
                        return new ConstantState(true);
                    }

                    if (left is ConstantState { Value: false })
                    {
                        return right;
                    }

                    if (right is ConstantState { Value: false })
                    {
                        return left;
                    }

                    return left == null && right == null ? null : Copied(expression, bit);
                }

            default:
                return Copied(expression, bit);
        }
    }

    [Pure]
    private CopiedState Copied(Expression expression, int bit)
    {
        // ReSharper disable once LocalVariableHidesMember
        var (source, fallbacks) = Collecting(() => Operand(expression));
        return new CopiedState(source, bit, fallbacks);
    }

    [Pure]
    private Condition ConditionOf(Expression expression)
    {
        // ReSharper disable once LocalVariableHidesMember
        var (condition, fallbacks) = Collecting(() => DescribeCondition(expression) ?? new Condition($"`{expression}`", $"not `{expression}`"));
        return fallbacks.Count == 0 ? condition : condition with { Fallbacks = fallbacks };
    }

    /// <summary>
    /// Runs a description, collecting the expressions it falls back to rather than recording them against the flag being
    /// described, for descriptions made before the flags they apply to are known.
    /// </summary>
    private (T Result, IReadOnlyList<string> Fallbacks) Collecting<T>(Func<T> describe)
    {
        var previous = collectingFallbacks;
        var collected = new List<string>();
        collectingFallbacks = collected;
        try
        {
            return (describe(), collected);
        }
        finally
        {
            collectingFallbacks = previous;
        }
    }

    [Pure]
    private FlagEffect DescribeState(Flag flag, FlagState state)
    {
        foreach (var fallback in state.Fallbacks)
        {
            fallbacks.Add(new KeyValuePair<string, string>(flag.Name, fallback));
        }

        return state switch
        {
            UnchangedState => FlagEffect.Unaffected(flag),
            ConstantState { Value: true } => new FlagEffect(flag, FlagEffectKind.Set, "Set."),
            ConstantState { Value: false } => new FlagEffect(flag, FlagEffectKind.Reset, "Reset."),
            _ => Changed(flag, Sentences(state))
        };
    }

    /// <summary>
    /// Describes a state as one or more sentences. A condition on a flag that selects a mode, e.g. the 6502's D flag, gets a
    /// sentence per mode.
    /// </summary>
    [Pure]
    private static string Sentences(FlagState state)
    {
        if (state is ConditionalState { Condition.Mode: { } mode } conditional && !IsSetOrReset(conditional))
        {
            return $"In {mode.Set} mode, {Phrase(conditional.WhenTrue)}. In {mode.Reset} mode, {Phrase(conditional.WhenFalse)}.";
        }

        return $"{Capitalise(Phrase(state))}.";
    }

    [Pure]
    private static bool IsSetOrReset(ConditionalState state) =>
        state is { WhenTrue: ConstantState, WhenFalse: ConstantState { } } and ({ WhenTrue: ConstantState { Value: true }, WhenFalse: ConstantState { Value: false } } or { WhenTrue: ConstantState { Value: false }, WhenFalse: ConstantState { Value: true } });

    /// <summary>
    /// Describes a state as a phrase without a capital or full stop, e.g. "set if A is zero, otherwise reset".
    /// </summary>
    [Pure]
    private static string Phrase(FlagState state)
    {
        switch (state)
        {
            case UnchangedState:
                return "unaffected";

            case ConstantState constant:
                return constant.Value ? "set" : "reset";

            case CopiedState copied:
                return $"copied from bit {copied.Bit} of {copied.Source}";

            case ConditionalState conditional:
                var (condition, whenTrue, whenFalse) = conditional;
                switch (whenTrue, whenFalse)
                {
                    case (ConstantState { Value: true }, ConstantState { Value: false }):
                        return $"set if {condition.Positive}, otherwise reset";

                    case (ConstantState { Value: false }, ConstantState { Value: true }):
                        return $"set if {condition.Negative}, otherwise reset";

                    // Two ifs that set the flag, e.g. the Z80's undocumented ANC sets C with N, set it if either condition holds.
                    case (ConstantState { Value: true }, ConditionalState { WhenTrue: ConstantState { Value: true }, WhenFalse: ConstantState { Value: false } } inner):
                        return $"set if {condition.Positive} or {inner.Condition.Positive}, otherwise reset";

                    case (ConditionalState { WhenTrue: ConstantState { Value: true }, WhenFalse: ConstantState { Value: false } } inner, ConstantState { Value: false }):
                        return $"set if {condition.Positive} and {inner.Condition.Positive}, otherwise reset";

                    case (_, UnchangedState):
                        return $"{Phrase(whenTrue)} if {condition.Positive}, otherwise unaffected";

                    case (UnchangedState, _):
                        return $"{Phrase(whenFalse)} if {condition.Negative}, otherwise unaffected";

                    default:
                        return $"{Phrase(whenTrue)} if {condition.Positive}, otherwise {Phrase(whenFalse)}";
                }

            default:
                throw new InvalidOperationException($"Unexpected state {state}.");
        }
    }

    [Pure]
    private FlagEffect DescribeAtCallSites(Flag flag, Expression expression)
    {
        describing = flag;
        if (callSites.Count == 0)
        {
            return Describe(flag, expression);
        }

        var effects = new List<(FlagEffect Effect, bool Completes)>();
        foreach (var callSite in callSites)
        {
            temporaries = callSite.Temporaries;
            describing = flag;
            effects.Add((Describe(flag, expression), callSite.Completes));
        }

        var distinct = effects.DistinctBy(e => e.Effect.Documentation, StringComparer.Ordinal).ToList();
        if (distinct.Count == 1)
        {
            return distinct[0].Effect;
        }

        // The paths differ, e.g. a repeating block instruction sets the undocumented flags from the program counter while it
        // repeats. Describe the completing path first.
        var completing = distinct.FirstOrDefault(e => e.Completes);
        var repeating = distinct.FirstOrDefault(e => !e.Completes);
        if (distinct.Count == 2 && completing.Effect != null && repeating.Effect != null)
        {
            return Changed(flag, $"When the instruction completes, {Lower(completing.Effect.Documentation)} When it repeats, {Lower(repeating.Effect.Documentation)}");
        }

        return Changed(flag, string.Join(" ", distinct.Select((e, index) => $"Path {index + 1}: {e.Effect.Documentation}")));
    }

    [Pure]
    private static string Lower(string description) => char.ToLowerInvariant(description[0]) + description[1..];

    [Pure]
    private static string Capitalise(string description) => char.ToUpperInvariant(description[0]) + description[1..];

    [Pure]
    private FlagEffect Describe(Flag flag, Expression expression)
    {
        switch (expression)
        {
            case Number { Value: 0 }:
                return new FlagEffect(flag, FlagEffectKind.Reset, "Reset.");

            case Number { Value: 1 }:
                return new FlagEffect(flag, FlagEffectKind.Set, "Set.");

            case TemporaryVariableAccess temporary when !temporaryNames.ContainsKey(temporary.Name) && temporaries.TryGetValue(temporary.Name, out var definition) && definition != null:
                return Describe(flag, definition);

            case Call { Function: var function, Arguments: var arguments }:
                return Changed(flag, DescribeCall(flag, function, arguments, expression));

            case FlagAccess flagAccess:
                return Changed(flag, $"Set to the previous value of the {flagAccess.Flag.Name} flag.");

            case DataMemberAccess dataMember:
                return Changed(flag, $"Set to the value of {Operand(dataMember)}.");

            case BinaryOperation binary:
                return Changed(flag, DescribeComparison(flag, binary) ?? Fallback(expression));

            default:
                return Changed(flag, Fallback(expression));
        }
    }

    [Pure]
    private static FlagEffect Changed(Flag flag, string description) => new(flag, FlagEffectKind.Changed, description);

    [Pure]
    private string DescribeCall(Flag flag, Function function, IReadOnlyList<Expression> arguments, Expression expression)
    {
        if (function == PreDefinedFunction.CopyFrom)
        {
            return $"Copied from bit {flag.Index} of {Operand(arguments[0])}.";
        }

        return DescribeCallCondition(function, arguments) is { } condition ? $"Set if {condition.Positive}, otherwise reset." : Fallback(expression);
    }

    [Pure]
    private Condition? DescribeCallCondition(Function function, IReadOnlyList<Expression> arguments)
    {
        if (function == PreDefinedFunction.IsZero)
        {
            return new Condition($"{Operand(arguments[0])} is zero", $"{Operand(arguments[0])} is not zero");
        }

        if (function is UserDefinedFunction { Documentation: { } documentation, NotDocumentation: { } notDocumentation })
        {
            return new Condition(Substitute(documentation, function, arguments), Substitute(notDocumentation, function, arguments));
        }

        return null;
    }

    /// <summary>
    /// Replaces the parameters in braces in a function's documentation with its arguments.
    /// </summary>
    [Pure]
    private string Substitute(string documentation, Function function, IReadOnlyList<Expression> arguments) =>
        PlaceholderRegex().Replace(documentation, match => Operand(arguments[function.Parameters.Index().First(parameter => parameter.Item == match.Groups[1].Value).Index]));

    [Pure]
    private string? DescribeComparison(Flag flag, BinaryOperation binary)
    {
        // A flag set from its own previous value compared with 0 is inverted.
        if (binary.Operator == Operator.Equality && Resolve(binary.Left) is FlagAccess { Flag: var compared } && compared == flag && Resolve(binary.Right) is Number { Value: 0 })
        {
            return "Inverted.";
        }

        return DescribeCondition(binary) is { } condition ? $"Set if {condition.Positive}, otherwise reset." : null;
    }

    /// <summary>
    /// Describes a boolean expression, e.g. the condition of an if, both as it is and negated. Returns <c>null</c> if the
    /// expression is not one that can be described as a condition.
    /// </summary>
    [Pure]
    private Condition? DescribeCondition(Expression expression)
    {
        switch (Resolve(expression))
        {
            case UnaryOperation { Operator: var @operator } unary when @operator == Operator.Not:
                return DescribeCondition(unary.Expression)?.Negate();

            case FlagAccess flagAccess:
                return new Condition($"the {flagAccess.Flag.Name} flag is set", $"the {flagAccess.Flag.Name} flag is reset", flagAccess.Flag.Mode);

            case Call { Function: var function, Arguments: var arguments }:
                return DescribeCallCondition(function, arguments);

            case BinaryOperation binary:
                return DescribeBinaryCondition(binary);

            default:
                return null;
        }
    }

    [Pure]
    private Condition? DescribeBinaryCondition(BinaryOperation binary)
    {
        var left = Resolve(binary.Left);
        var right = Resolve(binary.Right);

        if (binary.Operator == Operator.Equality || binary.Operator == Operator.NotEquals)
        {
            var isEquality = binary.Operator == Operator.Equality;
            var condition = DescribeEquality(left, right);
            return isEquality ? condition : condition.Negate();
        }

        if (binary.Operator == Operator.GreaterThan)
        {
            return new Condition($"{Operand(left)} is greater than {Operand(right)}", $"{Operand(left)} is less than or equal to {Operand(right)}");
        }

        if (binary.Operator == Operator.GreaterThanOrEqual)
        {
            return new Condition($"{Operand(left)} is greater than or equal to {Operand(right)}", $"{Operand(left)} is less than {Operand(right)}");
        }

        if (binary.Operator == Operator.LessThan)
        {
            return new Condition($"{Operand(left)} is less than {Operand(right)}", $"{Operand(left)} is greater than or equal to {Operand(right)}");
        }

        if (binary.Operator == Operator.LessThanOrEqual)
        {
            return new Condition($"{Operand(left)} is less than or equal to {Operand(right)}", $"{Operand(left)} is greater than {Operand(right)}");
        }

        return null;
    }

    /// <summary>
    /// Describes <c>left == right</c>.
    /// </summary>
    [Pure]
    private Condition DescribeEquality(Expression left, Expression right)
    {
        if (right is Number number)
        {
            // A boolean function compared with true, e.g. even_parity(x) == 1, is just the function; compared with false it is
            // the function negated.
            if (left is Call { Function: var function, Arguments: var arguments } && number.Value is 0 or 1 && DescribeCallCondition(function, arguments) is { } call)
            {
                return number.Value == 1 ? call : call.Negate();
            }

            if (left is BinaryOperation { Operator: var leftOperator, Right: Number mask, Left: var masked } && leftOperator == Operator.And)
            {
                // Bit tests: (x & mask) == mask and (x & mask) == 0.
                if (IsSingleBit(mask.Value) && number.Value is var value && (value == mask.Value || value == 0))
                {
                    var bit = BitIndex(mask.Value);
                    var set = new Condition($"bit {bit} of {Operand(masked)} is set", $"bit {bit} of {Operand(masked)} is reset");
                    return value == 0 ? set.Negate() : set;
                }

                if (mask.Value == 0xFF && number.Value == 0)
                {
                    return new Condition($"the low byte of {Operand(masked)} is zero", $"the low byte of {Operand(masked)} is not zero");
                }

                return new Condition($"{Operand(left)} equals {Operand(number)}", $"{Operand(left)} does not equal {Operand(number)}");
            }

            if (left is FlagAccess flagAccess && number.Value == 0)
            {
                return new Condition($"the {flagAccess.Flag.Name} flag was reset", $"the {flagAccess.Flag.Name} flag was set");
            }

            if (number.Value == 0)
            {
                return new Condition($"{Operand(left)} is zero", $"{Operand(left)} is not zero");
            }
        }

        return new Condition($"{Operand(left)} equals {Operand(right)}", $"{Operand(left)} does not equal {Operand(right)}");
    }

    /// <summary>
    /// Resolves a temporary that is not in the vocabulary to the expression that defines it.
    /// </summary>
    [Pure]
    private Expression Resolve(Expression expression)
    {
        while (expression is TemporaryVariableAccess temporary &&
               !temporaryNames.ContainsKey(temporary.Name) &&
               temporaries.TryGetValue(temporary.Name, out var definition) &&
               definition is not null and not RegisterAccess and not DataMemberAccess)
        {
            expression = definition;
        }

        return expression;
    }

    [Pure]
    private string Operand(Expression expression)
    {
        switch (expression)
        {
            case RegisterAccess register:
                return RegisterName(register.Register);

            case TemporaryVariableAccess temporary:
                return TemporaryName(temporary);

            case DataMemberAccess { DataMember: var member }:
                return member == PreDefinedDataMember.Data
                    ? assigned.Contains(member.FieldName) ? "the result" : "the value read"
                    : member.Name;

            case FlagAccess flagAccess:
                return $"the {flagAccess.Flag.Name} flag";

            case ConditionAccess condition:
                return $"condition {condition.Condition.Name}";

            case Number number:
                return number.NumberString;

            case Definitions.Language.Ast.Boolean boolean:
                return boolean.Value ? "true" : "false";

            case BinaryOperation binary:
                return $"({Operand(binary.Left)} {binary.Operator.Symbol} {Operand(binary.Right)})";

            case UnaryOperation unary:
                return $"{unary.Operator.Symbol}{Operand(unary.Expression)}";

            case Call { Function: var function, Arguments: var arguments }:
                if (function == PreDefinedFunction.Signed)
                {
                    return $"the signed value of {Operand(arguments[0])}";
                }

                if (function == PreDefinedFunction.IsZero)
                {
                    return $"whether {Operand(arguments[0])} is zero";
                }

                return function switch
                {
                    UserDefinedFunction { ValueDocumentation: { } value } => Substitute(value, function, arguments),
                    UserDefinedFunction { Type: DataType.Bool or DataType.I32Bool, Documentation: { } condition } => $"whether {Substitute(condition, function, arguments)}",
                    UserDefinedFunction { Documentation: { } value } => Substitute(value, function, arguments),
                    _ => Fallback(expression)
                };

            default:
                return Fallback(expression);
        }
    }

    [Pure]
    private string RegisterName(Register register) => placeholders.TryGetValue(register.Name, out var placeholder) ? placeholder : register.Name;

    [Pure]
    private string TemporaryName(TemporaryVariableAccess temporary)
    {
        if (temporaryNames.TryGetValue(temporary.Name, out var vocabulary))
        {
            return vocabulary;
        }

        if (!temporaries.TryGetValue(temporary.Name, out var definition) || definition == null)
        {
            return Fallback(temporary);
        }

        return definition switch
        {
            // A temporary holding a register that the instruction then changes holds the register's original value.
            RegisterAccess register => assigned.Contains(register.Register.Name) ? $"the original value of {RegisterName(register.Register)}" : RegisterName(register.Register),
            DataMemberAccess { DataMember: var member } when member == PreDefinedDataMember.Data => "the value read",
            _ => Operand(definition)
        };
    }

    [Pure]
    private string Fallback(Expression expression)
    {
        if (collectingFallbacks != null)
        {
            collectingFallbacks.Add(expression.ToString());
        }
        else
        {
            fallbacks.Add(new KeyValuePair<string, string>(describing?.Name ?? "", expression.ToString()));
        }

        return $"`{expression}`";
    }

    [Pure]
    private static bool IsSingleBit(int value) => value != 0 && (value & (value - 1)) == 0;

    [Pure]
    private static int BitIndex(int value) => int.TrailingZeroCount(value);

    private sealed record FlagsCallSite(IReadOnlyDictionary<string, Expression?> Temporaries, bool Completes);

    /// <summary>
    /// A boolean condition described both as it is and negated.
    /// </summary>
    /// <param name="Positive">The condition, e.g. "A is zero".</param>
    /// <param name="Negative">The negated condition, e.g. "A is not zero".</param>
    /// <param name="Mode">The modes the condition selects between, if it tests a flag that selects a mode.</param>
    private sealed record Condition(string Positive, string Negative, FlagMode? Mode = null)
    {
        public IReadOnlyList<string> Fallbacks { get; init; } = [];

        [Pure]
        public Condition Negate() => new(Negative, Positive, Mode) { Fallbacks = Fallbacks };

        public bool Equals(Condition? other) => other != null && Positive == other.Positive && Negative == other.Negative;

        public override int GetHashCode() => HashCode.Combine(Positive, Negative);
    }

    /// <summary>
    /// What a flag has been left as by the writes to the flags register so far on a path through the instruction.
    /// </summary>
    private abstract record FlagState
    {
        public virtual IReadOnlyList<string> Fallbacks => [];
    }

    private sealed record UnchangedState : FlagState
    {
        public static readonly UnchangedState Instance = new();
    }

    private sealed record ConstantState(bool Value) : FlagState;

    private sealed record CopiedState(string Source, int Bit, IReadOnlyList<string> CopyFallbacks) : FlagState
    {
        public override IReadOnlyList<string> Fallbacks => CopyFallbacks;

        public bool Equals(CopiedState? other) => other != null && Source == other.Source && Bit == other.Bit;

        public override int GetHashCode() => HashCode.Combine(Source, Bit);
    }

    private sealed record ConditionalState(Condition Condition, FlagState WhenTrue, FlagState WhenFalse) : FlagState
    {
        public override IReadOnlyList<string> Fallbacks => [.. Condition.Fallbacks, .. WhenTrue.Fallbacks, .. WhenFalse.Fallbacks];
    }

    /// <summary>
    /// The state of a path through an instruction: what each flag has been left as, and the temporaries in scope.
    /// </summary>
    private sealed class PathState
    {
        public Dictionary<string, FlagState> Flags { get; } = new(StringComparer.Ordinal);

        public Dictionary<string, Expression?> Temporaries { get; } = new(StringComparer.Ordinal);

        [Pure]
        public PathState Clone()
        {
            var clone = new PathState();
            foreach (var (flag, state) in Flags)
            {
                clone.Flags[flag] = state;
            }

            foreach (var (name, definition) in Temporaries)
            {
                clone.Temporaries[name] = definition;
            }

            return clone;
        }
    }

    /// <summary>
    /// What happens once the statements of a step are done: the instruction completes, moves to a named sequence, or carries
    /// on with the next step.
    /// </summary>
    private sealed record Continuation(string? MoveToSequence, bool Completes, IReadOnlySet<string> Visited, Func<PathState, PathState> Next);
}