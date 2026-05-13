using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract record StatementGenerationMode
{
    [Pure]
    public static StatementGenerationMode Normal { get; } = new NormalMode();

    [Pure]
    public static StatementGenerationMode Overlap { get; } = new OverlapMode();

    [Pure]
    public static StatementGenerationMode InstructionEmulator { get; } = new InstructionEmulatorMode();

    [Pure]
    public static StatementGenerationMode CreateInstructionStep(string? nextInstructionVariableName, Step? instructionExitOverlapStep, int instructionTStatesBeforeStep) =>
        new InstructionStepMode(new StatementGeneratorContext.InstructionStepState(nextInstructionVariableName, instructionExitOverlapStep, instructionTStatesBeforeStep));

    [Pure]
    public static StatementGenerationMode CreateInstructionCompletion(string instructionUpdatesFlagsParameterName) =>
        new InstructionCompletionMode(instructionUpdatesFlagsParameterName);

    public abstract SequenceTransitionTarget SequenceTransitionTarget { get; }

    public virtual bool ExecutesStoredOverlapOnStart => true;

    public virtual bool EmitsTrailingStatements => false;

    public virtual bool SkipHandleInterruptsCall => false;

    public virtual bool IsInstructionEmulatorMode => false;

    public virtual string? InstructionUpdatesFlagsParameterName => null;

    public virtual StatementGeneratorContext.InstructionStepState? InstructionStep => null;

    [Pure]
    private sealed record NormalMode : StatementGenerationMode
    {
        public override SequenceTransitionTarget SequenceTransitionTarget => SequenceTransitionTarget.CurrentStep;

        public override bool EmitsTrailingStatements => true;
    }

    [Pure]
    private sealed record OverlapMode : StatementGenerationMode
    {
        public override SequenceTransitionTarget SequenceTransitionTarget => SequenceTransitionTarget.CurrentStep;

        public override bool ExecutesStoredOverlapOnStart => false;

        public override bool SkipHandleInterruptsCall => true;
    }

    public sealed record InstructionEmulatorMode : StatementGenerationMode
    {
        public override SequenceTransitionTarget SequenceTransitionTarget => SequenceTransitionTarget.NextSequence;

        public override bool ExecutesStoredOverlapOnStart => false;

        public override bool IsInstructionEmulatorMode => true;
    }

    public sealed record InstructionStepMode(StatementGeneratorContext.InstructionStepState State) : StatementGenerationMode
    {
        public override SequenceTransitionTarget SequenceTransitionTarget => SequenceTransitionTarget.NextInstruction;

        public override bool ExecutesStoredOverlapOnStart => false;

        public override bool IsInstructionEmulatorMode => true;

        public override StatementGeneratorContext.InstructionStepState InstructionStep => State;
    }

    public sealed record InstructionCompletionMode(string UpdatesFlagsParameterName) : StatementGenerationMode
    {
        public override SequenceTransitionTarget SequenceTransitionTarget => SequenceTransitionTarget.NextSequence;

        public override string InstructionUpdatesFlagsParameterName => UpdatesFlagsParameterName;
    }
}