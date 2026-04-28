using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StatementGeneratorContext
{
    public StatementGeneratorContext(GeneratorContext generatorContext, Step? step)
        : this(generatorContext, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, false, null, StatementGenerationMode.Normal, null, null)
    {
    }

    private StatementGeneratorContext(
        GeneratorContext generatorContext,
        Step? step,
        HashSet<string> initializedTemporaryVariables,
        ImmutableDictionary<string, Expression> argumentScope,
        bool inBooleanContext,
        Expression? parent,
        StatementGenerationMode mode,
        InstructionStepState? instructionStep,
        string? instructionUpdatesFlagsParameterName)
    {
        if (mode == StatementGenerationMode.InstructionStep && instructionStep == null)
        {
            throw new InvalidOperationException("Instruction-step mode requires instruction-step state.");
        }

        if (mode != StatementGenerationMode.InstructionStep && instructionStep != null)
        {
            throw new InvalidOperationException("Instruction-step state is only valid in instruction-step mode.");
        }

        if (mode != StatementGenerationMode.InstructionCompletion && instructionUpdatesFlagsParameterName != null)
        {
            throw new InvalidOperationException("An instruction-updates-flags parameter is only valid in instruction-completion mode.");
        }

        GeneratorContext = generatorContext;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        InBooleanContext = inBooleanContext;
        Parent = parent;
        Mode = mode;
        InstructionStep = instructionStep;
        InstructionUpdatesFlagsParameterName = instructionUpdatesFlagsParameterName;
    }

    public GeneratorContext GeneratorContext { get; }

    public Configuration Configuration => GeneratorContext.Configuration;

    public Step? Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public bool InBooleanContext { get; }

    public Expression? Parent { get; }

    public StatementGenerationMode Mode { get; }

    public InstructionStepState? InstructionStep { get; }

    public string? InstructionUpdatesFlagsParameterName { get; }

    public bool SkipHandleInterrupts => Mode == StatementGenerationMode.Overlap;

    public bool InstructionCompletionMode => Mode == StatementGenerationMode.InstructionCompletion;

    public bool InstructionStepMode => Mode == StatementGenerationMode.InstructionStep;

    public bool InstructionEmulatorMode => Mode is StatementGenerationMode.InstructionEmulator or StatementGenerationMode.InstructionStep;

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
        With(mode: StatementGenerationMode.InstructionStep, instructionStep: new InstructionStepState(nextInstructionVariableName, instructionExitOverlapStep, instructionTStatesBeforeStep), updateInstructionStep: true);

    [Pure]
    public StatementGeneratorContext WithInstructionEmulatorMode() => With(mode: StatementGenerationMode.InstructionEmulator);

    [Pure]
    public StatementGeneratorContext WithInstructionCompletionMode(string instructionUpdatesFlagsParameterName) =>
        With(
            mode: StatementGenerationMode.InstructionCompletion,
            instructionUpdatesFlagsParameterName: instructionUpdatesFlagsParameterName,
            updateInstructionUpdatesFlagsParameterName: true);

    [Pure]
    public InstructionStepState RequiredInstructionStep => InstructionStep ?? throw new InvalidOperationException("Instruction-step state is only available in instruction-step mode.");

    [Pure]
    private StatementGeneratorContext With(
        HashSet<string>? initializedTemporaryVariables = null,
        ImmutableDictionary<string, Expression>? argumentScope = null,
        bool? inBooleanContext = null,
        Expression? parent = null,
        bool updateParent = false,
        StatementGenerationMode? mode = null,
        InstructionStepState? instructionStep = null,
        bool updateInstructionStep = false,
        string? instructionUpdatesFlagsParameterName = null,
        bool updateInstructionUpdatesFlagsParameterName = false)
    {
        var updatedMode = mode ?? Mode;
        var updatedInstructionStep = updateInstructionStep
            ? instructionStep
            : updatedMode == Mode
                ? InstructionStep
                : null;
        var updatedInstructionUpdatesFlagsParameterName = updateInstructionUpdatesFlagsParameterName
            ? instructionUpdatesFlagsParameterName
            : updatedMode == Mode
                ? InstructionUpdatesFlagsParameterName
                : null;

        return new StatementGeneratorContext(
            GeneratorContext,
            Step,
            initializedTemporaryVariables ?? InitializedTemporaryVariables,
            argumentScope ?? ArgumentScope,
            inBooleanContext ?? InBooleanContext,
            updateParent ? parent : Parent,
            updatedMode,
            updatedInstructionStep,
            updatedInstructionUpdatesFlagsParameterName);
    }

    public sealed record InstructionStepState(
        string? NextInstructionVariableName,
        Step? ExitOverlapStep,
        int TStatesBeforeStep);
}