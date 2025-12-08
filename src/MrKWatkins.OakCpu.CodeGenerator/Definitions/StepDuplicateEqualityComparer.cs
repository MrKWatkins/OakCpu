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
public sealed class StepDuplicateEqualityComparer : IEqualityComparer<Step>
{
    public static readonly StepDuplicateEqualityComparer Instance = new();

    private StepDuplicateEqualityComparer()
    {
    }

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

        if (x.NextOpcode != y.NextOpcode)
        {
            return false;
        }

        if (x.RequiresPrefixReset != y.RequiresPrefixReset)
        {
            return false;
        }

        // The remaining checks only apply if they're part of instructions.
        if (x.Sequence is not Instruction instructionX || y.Sequence is not Instruction instructionY)
        {
            return true;
        }

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

        return hashCode.ToHashCode();
    }
}