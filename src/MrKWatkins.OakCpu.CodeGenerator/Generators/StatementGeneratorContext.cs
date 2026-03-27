using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StatementGeneratorContext
{
    public StatementGeneratorContext(GeneratorContext generatorContext, Step? step)
        : this(generatorContext, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, false, null, false, null, false, null, null, false, 0)
    {
    }

    private StatementGeneratorContext(
        GeneratorContext generatorContext,
        Step? step,
        HashSet<string> initializedTemporaryVariables,
        ImmutableDictionary<string, Expression> argumentScope,
        bool inBooleanContext,
        Expression? parent,
        bool skipHandleInterrupts,
        string? stepCompleteLabel,
        bool instructionStepMode,
        string? nextInstructionVariableName,
        Step? instructionExitOverlapStep,
        bool instructionEmulatorMode,
        int instructionTStatesBeforeStep)
    {
        GeneratorContext = generatorContext;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        InBooleanContext = inBooleanContext;
        Parent = parent;
        SkipHandleInterrupts = skipHandleInterrupts;
        StepCompleteLabel = stepCompleteLabel;
        InstructionStepMode = instructionStepMode;
        NextInstructionVariableName = nextInstructionVariableName;
        InstructionExitOverlapStep = instructionExitOverlapStep;
        InstructionEmulatorMode = instructionEmulatorMode;
        InstructionTStatesBeforeStep = instructionTStatesBeforeStep;
    }

    public GeneratorContext GeneratorContext { get; }

    public Configuration Configuration => GeneratorContext.Configuration;

    public Step? Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public bool InBooleanContext { get; }

    public Expression? Parent { get; }

    public bool SkipHandleInterrupts { get; }

    public string? StepCompleteLabel { get; }

    public bool InstructionStepMode { get; }

    public string? NextInstructionVariableName { get; }

    public Step? InstructionExitOverlapStep { get; }

    public bool InstructionEmulatorMode { get; }

    public int InstructionTStatesBeforeStep { get; }

    [Pure]
    public StatementGeneratorContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)).Where(t => !ArgumentScope.ContainsKey(t.Key)));

        return new StatementGeneratorContext(GeneratorContext, Step, InitializedTemporaryVariables, newScope, InBooleanContext, Parent, SkipHandleInterrupts, StepCompleteLabel, InstructionStepMode, NextInstructionVariableName, InstructionExitOverlapStep, InstructionEmulatorMode, InstructionTStatesBeforeStep);
    }

    [Pure]
    public StatementGeneratorContext WithBooleanContext() =>
        new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, true, Parent, SkipHandleInterrupts, StepCompleteLabel, InstructionStepMode, NextInstructionVariableName, InstructionExitOverlapStep, InstructionEmulatorMode, InstructionTStatesBeforeStep);

    [Pure]
    public StatementGeneratorContext WithChildVariableScope() =>
        new(GeneratorContext, Step, [.. InitializedTemporaryVariables], ArgumentScope, InBooleanContext, Parent, SkipHandleInterrupts, StepCompleteLabel, InstructionStepMode, NextInstructionVariableName, InstructionExitOverlapStep, InstructionEmulatorMode, InstructionTStatesBeforeStep);

    [Pure]
    public StatementGeneratorContext WithParentExpression(Expression parent) =>
        new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, InBooleanContext, parent, SkipHandleInterrupts, StepCompleteLabel, InstructionStepMode, NextInstructionVariableName, InstructionExitOverlapStep, InstructionEmulatorMode, InstructionTStatesBeforeStep);

    [Pure]
    public StatementGeneratorContext WithoutHandleInterrupts() =>
        new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, InBooleanContext, Parent, true, StepCompleteLabel, InstructionStepMode, NextInstructionVariableName, InstructionExitOverlapStep, InstructionEmulatorMode, InstructionTStatesBeforeStep);

    [Pure]
    public StatementGeneratorContext WithInstructionStepMode(string? nextInstructionVariableName, Step? instructionExitOverlapStep, int instructionTStatesBeforeStep) =>
        new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, InBooleanContext, Parent, SkipHandleInterrupts, StepCompleteLabel, true, nextInstructionVariableName, instructionExitOverlapStep, true, instructionTStatesBeforeStep);

    [Pure]
    public StatementGeneratorContext WithInstructionEmulatorMode() =>
        new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, InBooleanContext, Parent, SkipHandleInterrupts, StepCompleteLabel, false, null, null, true, InstructionTStatesBeforeStep);
}
