using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class StepMetadata
{
    [Pure]
    internal static int GetNextStep(GeneratorContext context, Step step) =>
        context.GetStepLayout(step).NextOpcode switch
        {
            NextOpcodeMode.Read => 0,
            NextOpcodeMode.Overlapped when context.GetStepLayout(step).Sequence is PrefixJump => context.GetStepLayout(context.OpcodeRead.Steps[1]).Index,
            NextOpcodeMode.Overlapped => GetOverlappedNextStep(context, context.GetStepLayout(step).Sequence),
            NextOpcodeMode.Custom => context.ErrorStepIndex,
            NextOpcodeMode.Loop => context.GetStepLayout(context.GetStepLayout(step).Sequence.FirstStep).Index,
            null when context.GetStepLayout(step).QueuesOverlapStep => context.GetStepLayout(context.OpcodeRead.FirstStep).Index,
            null => context.GetStepLayout(step).Index + 1,
            _ => throw new NotSupportedException($"The {nameof(NextOpcodeMode)} {context.GetStepLayout(step).NextOpcode} is not supported.")
        };

    [Pure]
    internal static Action GetAction(GeneratorContext context, Step step) =>
        context.GetStepLayout(step) is { NextOpcode: NextOpcodeMode.Overlapped, Sequence: PrefixJump }
            ? context.GetStepLayout(context.OpcodeRead.FirstStep).RequiredAction
            : context.GetStepLayout(step).RequiredAction;

    [Pure]
    private static int GetOverlappedNextStep(GeneratorContext context, StepSequence sequence) =>
        sequence.OverlappedSequenceName == null
            ? context.GetStepLayout(context.OpcodeRead.FirstStep).Index
            : context.GetStepLayout(context.GetSequence(sequence.OverlappedSequenceName).FirstStep).Index;
}