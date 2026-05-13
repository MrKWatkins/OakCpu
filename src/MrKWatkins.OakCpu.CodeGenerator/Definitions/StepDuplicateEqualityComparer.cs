using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

/// <summary>
/// Two steps are considered duplicates if:
/// * They have the same statements.
/// * Their NextOpcodes are the same.
/// * Whether they require an opcode prefix reset or not is the same.
/// * If they are instructions, then whether they update flags or not must be equal.
/// * If they are instructions, and if they update flags, then the flags implementations must also be equal.
/// </summary>
internal sealed class StepDuplicateEqualityComparer(IReadOnlyDictionary<Step, StepSequence> stepSequences) : IEqualityComparer<Step>
{
    public bool Equals(Step? x, Step? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (!x.Statements.SequenceEqual(y.Statements, StatementDuplicateEqualityComparer.Instance))
        {
            return false;
        }

        var xSequence = GetSequence(x);
        var ySequence = GetSequence(y);

        if (StepLayout.GetNextOpcode(x, xSequence) != StepLayout.GetNextOpcode(y, ySequence))
        {
            return false;
        }

        if (StepLayout.GetRequiresPrefixReset(x, xSequence) != StepLayout.GetRequiresPrefixReset(y, ySequence))
        {
            return false;
        }

        if (StepLayout.GetExecutesStoredOverlapOnStart(x, xSequence) != StepLayout.GetExecutesStoredOverlapOnStart(y, ySequence))
        {
            return false;
        }

        if (StepLayout.GetQueuesOverlapStep(x, xSequence) != StepLayout.GetQueuesOverlapStep(y, ySequence))
        {
            return false;
        }

        if (xSequence.OverlappedSequenceName != ySequence.OverlappedSequenceName)
        {
            return false;
        }

        var xIsInstruction = xSequence is Instruction;
        var yIsInstruction = ySequence is Instruction;
        if (xIsInstruction != yIsInstruction)
        {
            return false;
        }

        // The remaining checks only apply if they're part of instructions.
        if (!xIsInstruction)
        {
            return true;
        }

        var instructionX = (Instruction)xSequence;
        var instructionY = (Instruction)ySequence;

        if (instructionX.UpdatesFlags != instructionY.UpdatesFlags)
        {
            return false;
        }

        // Do any of the statements have a flags call? Only instructions can have flags calls. If they don't, they're equal.
        if (x.Statements.SelectMany(s => s.TraverseDepthFirst()).OfType<Call>().All(call => call.Function != PreDefinedFunction.Flags))
        {
            return true;
        }

        // They're equal if the flags expressions are equal.
        return InstructionFlagsDuplicateEqualityComparer.Instance.Equals(instructionX, instructionY);
    }

    public int GetHashCode(Step obj)
    {
        var hashCode = new HashCode();
        foreach (var statement in obj.Statements)
        {
            hashCode.Add(StatementDuplicateEqualityComparer.Instance.GetHashCode(statement));
        }

        var sequence = GetSequence(obj);

        hashCode.Add(StepLayout.GetNextOpcode(obj, sequence));
        hashCode.Add(StepLayout.GetRequiresPrefixReset(obj, sequence));
        hashCode.Add(StepLayout.GetExecutesStoredOverlapOnStart(obj, sequence));
        hashCode.Add(StepLayout.GetQueuesOverlapStep(obj, sequence));
        hashCode.Add(sequence.OverlappedSequenceName);
        hashCode.Add(sequence is Instruction);

        return hashCode.ToHashCode();
    }

    [Pure]
    private StepSequence GetSequence(Step step) => stepSequences.TryGetValue(step, out var sequence) ? sequence : throw new InvalidOperationException($"No sequence has been defined for step {step.Name}.");
}