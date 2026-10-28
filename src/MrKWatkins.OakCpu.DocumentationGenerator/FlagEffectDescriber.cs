using System.Collections.Frozen;
using System.Globalization;
using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Describes the effect an instruction has on each flag in prose, from the expressions the instruction uses to set them.
/// </summary>
public sealed class FlagEffectDescriber
{
    private static readonly FrozenDictionary<string, string> TemporaryVocabulary = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["result"] = "the result",
        ["result16"] = "the 16-bit result",
        ["high"] = "the high byte of the result",
        ["comparison"] = "the tested bit",
        ["sum"] = "the sum",
        ["difference"] = "the difference"
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, string> FunctionEffects = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["did_8_bit_addition_overflow"] = "Set if the addition overflowed, otherwise reset.",
        ["did_16_bit_addition_overflow"] = "Set if the addition overflowed, otherwise reset.",
        ["did_8_bit_subtraction_overflow"] = "Set if the subtraction overflowed, otherwise reset.",
        ["did_16_bit_subtraction_overflow"] = "Set if the subtraction overflowed, otherwise reset.",
        ["did_8_bit_addition_half_carry"] = "Set if there was a carry from bit 3 to bit 4, otherwise reset.",
        ["did_16_bit_addition_half_carry"] = "Set if there was a carry from bit 11 to bit 12, otherwise reset.",
        ["did_8_bit_subtraction_half_borrow"] = "Set if there was a borrow from bit 4, otherwise reset.",
        ["did_16_bit_subtraction_half_borrow"] = "Set if there was a borrow from bit 12, otherwise reset."
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, string> FunctionNouns = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["did_8_bit_addition_overflow"] = "the overflow",
        ["did_16_bit_addition_overflow"] = "the overflow",
        ["did_8_bit_subtraction_overflow"] = "the overflow",
        ["did_16_bit_subtraction_overflow"] = "the overflow",
        ["did_8_bit_addition_half_carry"] = "the half carry",
        ["did_16_bit_addition_half_carry"] = "the half carry",
        ["did_8_bit_subtraction_half_borrow"] = "the half borrow",
        ["did_16_bit_subtraction_half_borrow"] = "the half borrow",
        ["even_parity"] = "the parity",
        ["is_zero"] = "whether {0} is zero",
        ["is_negative"] = "whether {0} is negative"
    }.ToFrozenDictionary();

    private readonly Configuration configuration;
    private readonly Instruction instruction;
    private readonly IReadOnlyDictionary<string, string> placeholders;
    private readonly IReadOnlyList<FlagsCallSite> callSites;
    private readonly IReadOnlySet<string> assigned;
    private readonly List<KeyValuePair<string, string>> fallbacks = [];
    private Flag? describing;
    private IReadOnlyDictionary<string, Expression> temporaries = new Dictionary<string, Expression>();

    /// <param name="configuration">The CPU's configuration.</param>
    /// <param name="instruction">The instruction.</param>
    /// <param name="placeholders">
    /// The placeholders for the instruction's operands, keyed by the operand, e.g. <c>B</c> = <c>{R0}</c>, so that the
    /// descriptions refer to placeholders rather than the operands of one opcode.
    /// </param>
    public FlagEffectDescriber(Configuration configuration, Instruction instruction, IReadOnlyDictionary<string, string> placeholders)
    {
        this.configuration = configuration;
        this.instruction = instruction;
        this.placeholders = placeholders;

        callSites = FindFlagsCallSites(instruction.Steps);

        assigned = instruction.Steps
            .SelectMany(step => step.Statements)
            .SelectMany(statement => statement.TraverseDepthFirst())
            .OfType<Assignment>()
            .Where(assignment => assignment.Target is not TemporaryVariableAccess)
            .Select(assignment => assignment.Target.Name)
            .ToHashSet(StringComparer.Ordinal);
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
        var definitions = new Dictionary<string, Expression>(StringComparer.Ordinal);
        foreach (var step in steps)
        {
            FindFlagsCallSites(step.Statements, definitions, false, callSites);
        }

        return callSites;
    }

    private static void FindFlagsCallSites(IReadOnlyList<Statement> statements, Dictionary<string, Expression> definitions, bool completes, List<FlagsCallSite> callSites)
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
                    callSites.Add(new FlagsCallSite(new Dictionary<string, Expression>(definitions, StringComparer.Ordinal), completes));
                    break;

                case IfStatement ifStatement:
                    var ifDefinitions = new Dictionary<string, Expression>(definitions, StringComparer.Ordinal);
                    FindFlagsCallSites(ifStatement.IfStatements, ifDefinitions, completes, callSites);
                    var elseDefinitions = new Dictionary<string, Expression>(definitions, StringComparer.Ordinal);
                    FindFlagsCallSites(ifStatement.ElseStatements, elseDefinitions, completes, callSites);

                    // A temporary assigned in a branch has a value that depends on the condition after the if, so cannot be
                    // described by either assignment.
                    foreach (var name in ifDefinitions.Keys.Concat(elseDefinitions.Keys).Distinct(StringComparer.Ordinal).ToList())
                    {
                        var before = definitions.GetValueOrDefault(name);
                        if (!ReferenceEquals(ifDefinitions.GetValueOrDefault(name), before) || !ReferenceEquals(elseDefinitions.GetValueOrDefault(name), before))
                        {
                            definitions.Remove(name);
                        }
                    }
                    break;
            }
        }
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
            .Select(flag => instruction.Flags.TryGetValue(flag.Name, out var expression) ? DescribeAtCallSites(flag, expression) : FlagEffect.Unaffected(flag))
            .ToList();

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

        var distinct = effects.DistinctBy(e => e.Effect.Description, StringComparer.Ordinal).ToList();
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
            return Changed(flag, $"When the instruction completes, {Lower(completing.Effect.Description)} When it repeats, {Lower(repeating.Effect.Description)}");
        }

        return Changed(flag, string.Join(" ", distinct.Select((e, index) => $"Path {index + 1}: {e.Effect.Description}")));
    }

    [Pure]
    private static string Lower(string description) => char.ToLowerInvariant(description[0]) + description[1..];

    [Pure]
    private FlagEffect Describe(Flag flag, Expression expression)
    {
        switch (expression)
        {
            case Number { Value: 0 }:
                return new FlagEffect(flag, FlagEffectKind.Reset, "Reset.");

            case Number { Value: 1 }:
                return new FlagEffect(flag, FlagEffectKind.Set, "Set.");

            case TemporaryVariableAccess temporary when !TemporaryVocabulary.ContainsKey(temporary.Name) && temporaries.TryGetValue(temporary.Name, out var definition):
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

        if (function == PreDefinedFunction.IsZero)
        {
            return $"Set if {Operand(arguments[0])} is zero, otherwise reset.";
        }

        return function.Name switch
        {
            "even_parity" => $"Set if {Operand(arguments[0])} has even parity, otherwise reset.",
            "is_negative" => $"Set if bit 7 of {Operand(arguments[0])} is set, otherwise reset.",
            _ => FunctionEffects.TryGetValue(function.Name, out var effect) ? effect : Fallback(expression)
        };
    }

    [Pure]
    private string? DescribeComparison(Flag flag, BinaryOperation binary)
    {
        var left = Resolve(binary.Left);
        var right = Resolve(binary.Right);

        if (binary.Operator == Operator.Equality || binary.Operator == Operator.NotEquals)
        {
            var isEquality = binary.Operator == Operator.Equality;
            if (right is Number number)
            {
                // A boolean function compared with true, e.g. even_parity(x) == 1, is just the function.
                if (left is Call { Function: var function, Arguments: var arguments } && (isEquality ? number.Value == 1 : number.Value == 0))
                {
                    return DescribeCall(flag, function, arguments, left);
                }

                // Bit tests: (x & mask) == mask, (x & mask) != 0 and (x & mask) == 0.
                if (left is BinaryOperation { Operator: var leftOperator, Right: Number mask } and { Left: var masked } && leftOperator == Operator.And && IsSingleBit(mask.Value))
                {
                    var bit = BitIndex(mask.Value);
                    var set = number.Value == mask.Value ? isEquality : number.Value == 0 ? !isEquality : throw new InvalidOperationException($"Unexpected bit test {binary}.");
                    return $"Set if bit {bit} of {Operand(masked)} is {(set ? "set" : "reset")}, otherwise reset.";
                }

                if (left is BinaryOperation { Operator: var maskOperator, Right: Number } and { Left: var value } && maskOperator == Operator.And)
                {
                    return $"Set if {Operand(left)} {(isEquality ? "equals" : "does not equal")} {Operand(number)}, otherwise reset.";
                }

                if (left is FlagAccess flagAccess && number.Value == 0)
                {
                    return flagAccess.Flag == flag
                        ? "Inverted."
                        : $"Set if the {flagAccess.Flag.Name} flag was {(isEquality ? "reset" : "set")}, otherwise reset.";
                }

                if (number.Value == 0)
                {
                    return $"Set if {Operand(left)} is {(isEquality ? "zero" : "not zero")}, otherwise reset.";
                }

                return $"Set if {Operand(left)} {(isEquality ? "equals" : "does not equal")} {Operand(number)}, otherwise reset.";
            }

            return $"Set if {Operand(left)} {(isEquality ? "equals" : "does not equal")} {Operand(right)}, otherwise reset.";
        }

        if (binary.Operator == Operator.GreaterThan)
        {
            return $"Set if {Operand(left)} is greater than {Operand(right)}, otherwise reset.";
        }

        if (binary.Operator == Operator.GreaterThanOrEqual)
        {
            return $"Set if {Operand(left)} is greater than or equal to {Operand(right)}, otherwise reset.";
        }

        if (binary.Operator == Operator.LessThan)
        {
            return $"Set if {Operand(left)} is less than {Operand(right)}, otherwise reset.";
        }

        if (binary.Operator == Operator.LessThanOrEqual)
        {
            return $"Set if {Operand(left)} is less than or equal to {Operand(right)}, otherwise reset.";
        }

        return null;
    }

    /// <summary>
    /// Resolves a temporary that is not in the vocabulary to the expression that defines it.
    /// </summary>
    [Pure]
    private Expression Resolve(Expression expression)
    {
        while (expression is TemporaryVariableAccess temporary &&
               !TemporaryVocabulary.ContainsKey(temporary.Name) &&
               temporaries.TryGetValue(temporary.Name, out var definition) &&
               definition is not RegisterAccess and not DataMemberAccess)
        {
            expression = definition;
        }

        return expression;
    }

    [Pure]
    private string Operand(Expression expression) => OperandCore(expression);

    [Pure]
    private string OperandCore(Expression expression)
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
                return $"({OperandCore(binary.Left)} {binary.Operator.Symbol} {OperandCore(binary.Right)})";

            case UnaryOperation unary:
                return $"{unary.Operator.Symbol}{OperandCore(unary.Expression)}";

            case Call { Function: var function, Arguments: var arguments }:
                if (function == PreDefinedFunction.Signed)
                {
                    return $"the signed value of {Operand(arguments[0])}";
                }

                if (FunctionNouns.TryGetValue(function.Name, out var noun))
                {
                    return string.Format(CultureInfo.InvariantCulture, noun, arguments.Select(Operand).ToArray<object>());
                }

                return Fallback(expression);

            default:
                return Fallback(expression);
        }
    }

    [Pure]
    private string RegisterName(Register register) => placeholders.TryGetValue(register.Name, out var placeholder) ? placeholder : register.Name;

    [Pure]
    private string TemporaryName(TemporaryVariableAccess temporary)
    {
        if (TemporaryVocabulary.TryGetValue(temporary.Name, out var vocabulary))
        {
            return vocabulary;
        }

        if (!temporaries.TryGetValue(temporary.Name, out var definition))
        {
            return Fallback(temporary);
        }

        return definition switch
        {
            // A temporary holding a register that the instruction then changes holds the register's original value.
            RegisterAccess register => assigned.Contains(register.Register.Name) ? $"the original value of {RegisterName(register.Register)}" : RegisterName(register.Register),
            DataMemberAccess { DataMember: var member } when member == PreDefinedDataMember.Data => "the value read",
            _ => OperandCore(definition)
        };
    }

    [Pure]
    private string Fallback(Expression expression)
    {
        fallbacks.Add(new KeyValuePair<string, string>(describing?.Name ?? "", expression.ToString()));
        return $"`{expression}`";
    }

    [Pure]
    private static bool IsSingleBit(int value) => value != 0 && (value & (value - 1)) == 0;

    [Pure]
    private static int BitIndex(int value) => int.TrailingZeroCount(value);

    private sealed record FlagsCallSite(IReadOnlyDictionary<string, Expression> Temporaries, bool Completes);
}