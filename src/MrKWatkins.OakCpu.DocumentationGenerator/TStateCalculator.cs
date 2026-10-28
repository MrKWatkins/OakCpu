using MrKWatkins.OakCpu.Definitions.Language.Ast;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Calculates the T-states instructions take from their steps.
/// </summary>
/// <remarks>
/// Every step of an instruction takes one T-state, except the last step of an instruction that overlaps with the next opcode
/// fetch, which runs during the fetch's first T-state, or that requests the next opcode fetch itself. The opcode fetch takes
/// the T-states of the <c>opcode_read</c> sequence, once for the opcode and once more for each prefix byte. Opcodes in a custom opcode table, such as the Z80's
/// <c>DDCB</c> table, are reached through a redirect instruction whose steps read the rest of the opcode, so they also pay
/// for the redirect.
/// </remarks>
public sealed class TStateCalculator
{
    private readonly int fetchTStates;
    private readonly IReadOnlyDictionary<string, Instruction> redirects;

    public TStateCalculator(CpuModel model)
    {
        fetchTStates = model.OpcodeRead.Steps.Count;
        redirects = model.Instructions
            .Select(instruction => (Instruction: instruction, Table: GetRedirectTable(instruction)))
            .Where(x => x.Table != null)
            .ToDictionary(x => x.Table!, x => x.Instruction, StringComparer.Ordinal);
    }

    /// <summary>
    /// Calculates the T-states an instruction takes for each of the paths through it, shortest first. An instruction that
    /// calls <c>instruction_complete()</c> conditionally, such as a conditional jump or a repeating block instruction, has
    /// a path that finishes at that step as well as the path through all of its steps.
    /// </summary>
    [Pure]
    public IReadOnlyList<int> Calculate(Instruction instruction)
    {
        var before = TStatesBefore(instruction);
        var exits = new SortedSet<int>();
        var total = CountTStates(instruction);
        for (var index = 0; index < instruction.Steps.Count; index++)
        {
            var completions = new List<bool>();
            FindInstructionCompletions(instruction.Steps[index].Statements, false, completions);
            if (completions.Any(conditional => conditional))
            {
                exits.Add(before + index + 1);
            }

            if (completions.Any(conditional => !conditional))
            {
                // The instruction always finishes here; any later steps are never executed.
                total = index + 1;
                break;
            }
        }

        exits.Add(before + total);
        return [.. exits];
    }

    /// <summary>
    /// Whether an instruction only redirects to another opcode table, e.g. the Z80's <c>DD CB</c> prefix, and so is not an
    /// instruction in its own right.
    /// </summary>
    [Pure]
    public static bool IsRedirect(Instruction instruction) => SetOpcodeStepTableCalls(instruction).Any();

    [Pure]
    private static string? GetRedirectTable(Instruction instruction) =>
        SetOpcodeStepTableCalls(instruction)
            .Select(call => call.Arguments.FirstOrDefault() as OpcodeStepTableAccess)
            .FirstOrDefault(access => access?.OpcodeStepTable.CustomName != null)?.OpcodeStepTable.CustomName;

    [Pure]
    private static IEnumerable<Call> SetOpcodeStepTableCalls(Instruction instruction) =>
        instruction.Steps
            .SelectMany(step => step.Statements)
            .SelectMany(statement => statement.TraverseDepthFirst())
            .OfType<Call>()
            .Where(call => call.Function == PreDefinedFunction.SetOpcodeStepTable);

    [Pure]
    private int TStatesBefore(Instruction instruction)
    {
        if (instruction.OpcodeTable != null)
        {
            if (!redirects.TryGetValue(instruction.OpcodeTable, out var redirect))
            {
                throw new InvalidOperationException($"No redirect instruction was found for opcode table {instruction.OpcodeTable}.");
            }

            return TStatesBefore(redirect) + CountTStates(redirect);
        }

        return fetchTStates * (instruction.Prefix.HasValue ? 2 : 1);
    }

    /// <summary>
    /// Counts the T-states a sequence's steps take. The last step takes none if it overlaps with the next opcode fetch, either
    /// because the sequence is overlapped or because the step requests the fetch itself, as the Z80's <c>EI</c> does to keep
    /// interrupts from being handled before the following instruction.
    /// </summary>
    [Pure]
    private static int CountTStates(StepSequence sequence) =>
        sequence.Steps.Count - (sequence.NextOpcode == NextOpcodeMode.Overlapped || RequestsOpcodeRead(sequence.Steps[^1]) ? 1 : 0);

    [Pure]
    private static bool RequestsOpcodeRead(Step step) => step.SpecifiedAction?.Name == "opcode_read";

    private static void FindInstructionCompletions(IReadOnlyList<Statement> statements, bool conditional, List<bool> completions)
    {
        foreach (var statement in statements)
        {
            switch (statement)
            {
                case CallStatement { Call.Function: var function } when function == PreDefinedFunction.InstructionComplete:
                    completions.Add(conditional);
                    break;

                case IfStatement ifStatement:
                    FindInstructionCompletions(ifStatement.IfStatements, true, completions);
                    FindInstructionCompletions(ifStatement.ElseStatements, true, completions);
                    break;
            }
        }
    }
}