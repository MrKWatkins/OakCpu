using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StatementGeneratorContext
{
    public StatementGeneratorContext(FileGeneratorContext fileContext, Step? step)
        : this(fileContext, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, false, null, StatementGenerationMode.Normal)
    {
    }

    private StatementGeneratorContext(
        FileGeneratorContext fileContext,
        Step? step,
        HashSet<string> initializedTemporaryVariables,
        ImmutableDictionary<string, Expression> argumentScope,
        bool inBooleanContext,
        Expression? parent,
        StatementGenerationMode mode)
    {
        FileContext = fileContext;
        GeneratorContext = fileContext.GeneratorContext;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        InBooleanContext = inBooleanContext;
        Parent = parent;
        Mode = mode;
    }

    internal FileGeneratorContext FileContext { get; }

    public GeneratorContext GeneratorContext { get; }

    internal RequiredUsings RequiredUsings => FileContext.RequiredUsings;

    public Configuration Configuration => GeneratorContext.Configuration;

    public Step? Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public bool InBooleanContext { get; }

    public Expression? Parent { get; }

    public StatementGenerationMode Mode { get; }

    public InstructionStepState? InstructionStep => Mode.InstructionStep;

    public string? InstructionUpdatesFlagsParameterName => Mode.InstructionUpdatesFlagsParameterName;

    public bool SkipHandleInterrupts => Mode.SkipHandleInterruptsCall;

    public bool InstructionCompletionMode => Mode is StatementGenerationMode.InstructionCompletionMode;

    public bool InstructionStepMode => Mode is StatementGenerationMode.InstructionStepMode;

    public bool InstructionEmulatorMode => Mode.IsInstructionEmulatorMode;

    [Pure]
    public StatementGeneratorContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)).Where(t => !ArgumentScope.ContainsKey(t.Key)));

        return With(argumentScope: newScope);
    }

    [Pure]
    public StatementGeneratorContext WithBooleanContext() => With(inBooleanContext: true);

    [Pure]
    public StatementGeneratorContext WithChildVariableScope() => With(initializedTemporaryVariables: [.. InitializedTemporaryVariables]);

    [Pure]
    public StatementGeneratorContext WithParentExpression(Expression parent) => With(parent: parent, updateParent: true);

    [Pure]
    public StatementGeneratorContext WithoutHandleInterrupts() => With(mode: StatementGenerationMode.Overlap);

    [Pure]
    public StatementGeneratorContext WithInstructionStepMode(string? nextInstructionVariableName, Step? instructionExitOverlapStep, int instructionTStatesBeforeStep) =>
        With(mode: StatementGenerationMode.CreateInstructionStep(nextInstructionVariableName, instructionExitOverlapStep, instructionTStatesBeforeStep));

    [Pure]
    public StatementGeneratorContext WithInstructionEmulatorMode() => With(mode: StatementGenerationMode.InstructionEmulator);

    [Pure]
    public StatementGeneratorContext WithInstructionCompletionMode(string instructionUpdatesFlagsParameterName) =>
        With(mode: StatementGenerationMode.CreateInstructionCompletion(instructionUpdatesFlagsParameterName));

    [Pure]
    public InstructionStepState RequiredInstructionStep => InstructionStep ?? throw new InvalidOperationException("Instruction-step state is only available in instruction-step mode.");

    [Pure]
    private StatementGeneratorContext With(
        HashSet<string>? initializedTemporaryVariables = null,
        ImmutableDictionary<string, Expression>? argumentScope = null,
        bool? inBooleanContext = null,
        Expression? parent = null,
        bool updateParent = false,
        StatementGenerationMode? mode = null)
    {
        return new StatementGeneratorContext(
            FileContext,
            Step,
            initializedTemporaryVariables ?? InitializedTemporaryVariables,
            argumentScope ?? ArgumentScope,
            inBooleanContext ?? InBooleanContext,
            updateParent ? parent : Parent,
            mode ?? Mode);
    }

    public sealed record InstructionStepState(
        string? NextInstructionVariableName,
        Step? ExitOverlapStep,
        int TStatesBeforeStep);
}