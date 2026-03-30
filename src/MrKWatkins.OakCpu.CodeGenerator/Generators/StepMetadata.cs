using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class StepMetadata
{
    [Pure]
    public static int GetNextStep(GeneratorContext context, Step step) =>
        step.NextOpcode switch
        {
            NextOpcodeMode.Read => 0,
            NextOpcodeMode.Overlapped when step.Sequence is PrefixJump => context.OpcodeRead.Steps[1].Index,
            NextOpcodeMode.Overlapped => GetOverlappedNextStep(context, step.Sequence),
            NextOpcodeMode.Custom => context.ErrorStepIndex,
            NextOpcodeMode.Loop => step.Sequence.FirstStep.Index,
            null when step.QueuesOverlapStep => context.OpcodeRead.FirstStep.Index,
            null => step.Index + 1,
            _ => throw new NotSupportedException($"The {nameof(NextOpcodeMode)} {step.NextOpcode} is not supported.")
        };

    [Pure]
    public static Action GetAction(GeneratorContext context, Step step) =>
        step is { NextOpcode: NextOpcodeMode.Overlapped, Sequence: PrefixJump }
            ? context.OpcodeRead.FirstStep.RequiredAction
            : step.RequiredAction;

    [Pure]
    private static int GetOverlappedNextStep(GeneratorContext context, StepSequence sequence) =>
        sequence.OverlappedSequenceName == null
            ? context.OpcodeRead.FirstStep.Index
            : context.GetSequence(sequence.OverlappedSequenceName).FirstStep.Index;
}