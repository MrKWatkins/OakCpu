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

    /// <summary>
    /// Gets the pins asserted during the step's T-state, sampled at the rising edge of its clock. Explicit sequence pins take
    /// precedence; otherwise the pins come from the patterns of the actions requested within the owning sequence.
    /// </summary>
    [Pure]
    internal static byte GetPins(GeneratorContext context, Step step)
    {
        if (!context.Configuration.HasPins)
        {
            return 0;
        }

        var layout = context.GetStepLayout(step);
        if (layout is { NextOpcode: NextOpcodeMode.Overlapped, Sequence: PrefixJump })
        {
            // Prefix jumps act as the first T-state of the next opcode fetch.
            return GetPins(context, context.OpcodeRead.FirstStep);
        }

        var sequence = layout.Sequence;
        var index = GetIndexInSequence(sequence, step);

        if (sequence.OverrideStepPins is { } overrideStepPins)
        {
            return index < overrideStepPins.Count ? overrideStepPins[index] : (byte)0;
        }

        byte pins = 0;
        for (var requestIndex = 0; requestIndex < sequence.Steps.Count; requestIndex++)
        {
            if (sequence.Steps[requestIndex].SpecifiedAction is not { PinPattern: { } pattern })
            {
                continue;
            }

            // The pattern can extend beyond the sequence's steps, e.g. EI starts the next opcode fetch itself and jumps into the
            // shared opcode read sequence, whose steps carry the rest of the cycle's pins.
            var offset = index - requestIndex;
            if (offset >= 0 && offset < pattern.Count)
            {
                pins |= pattern[offset];
            }
        }

        return pins;
    }

    [Pure]
    private static int GetIndexInSequence(StepSequence sequence, Step step)
    {
        for (var index = 0; index < sequence.Steps.Count; index++)
        {
            if (ReferenceEquals(sequence.Steps[index], step))
            {
                return index;
            }
        }

        throw new InvalidOperationException($"The step {step.Name} does not belong to the sequence {sequence.Name}.");
    }

    [Pure]
    private static int GetOverlappedNextStep(GeneratorContext context, StepSequence sequence) =>
        sequence.OverlappedSequenceName == null
            ? context.GetStepLayout(context.OpcodeRead.FirstStep).Index
            : context.GetStepLayout(context.GetSequence(sequence.OverlappedSequenceName).FirstStep).Index;
}