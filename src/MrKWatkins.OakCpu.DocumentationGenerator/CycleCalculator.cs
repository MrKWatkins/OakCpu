using MrKWatkins.OakCpu.Definitions.Language.Ast;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Calculates the cycles instructions take from their steps, following the paths through them: an instruction can finish
/// early with <c>instruction_complete()</c>, or carry on in a named sequence with <c>move_to_sequence()</c>.
/// </summary>
/// <remarks>
/// What a step costs depends on how the CPU counts its <see cref="Cycles" />. The opcode fetch costs the steps of the <c>opcode_read</c>
/// sequence, once for the opcode and once more for each prefix byte, and opcodes in a custom opcode table, such as the Z80's
/// <c>DDCB</c> table, also pay for the redirect instruction whose steps read the rest of the opcode.
/// </remarks>
public sealed class CycleCalculator
{
    private readonly CpuModel model;
    private readonly Cycles cycles;
    private readonly int fetchCycles;
    private readonly IReadOnlyDictionary<string, Instruction> redirects;

    public CycleCalculator(CpuModel model)
    {
        this.model = model;
        cycles = model.Cpu.Cycles;
        fetchCycles = model.OpcodeRead.Steps.Select((step, index) => StepCost(model.OpcodeRead, step, index)).Sum();
        redirects = model.Instructions
            .Select(instruction => (Instruction: instruction, Table: GetRedirectTable(instruction)))
            .Where(x => x.Table != null)
            .ToDictionary(x => x.Table!, x => x.Instruction, StringComparer.Ordinal);
    }

    /// <summary>
    /// Calculates the cycles an instruction takes for each of the paths through it, shortest first. A conditional jump or a
    /// repeating block instruction has a path for each way it can finish; everything else has one.
    /// </summary>
    [Pure]
    public IReadOnlyList<int> Calculate(Instruction instruction)
    {
        var before = CyclesBefore(instruction);
        var paths = new SortedSet<int>();
        foreach (var path in Paths(instruction, 0, new HashSet<StepSequence>()))
        {
            paths.Add(Math.Max(before + path, cycles.Minimum));
        }

        return [.. paths];
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
    private int CyclesBefore(Instruction instruction)
    {
        if (instruction.OpcodeTable != null)
        {
            if (!redirects.TryGetValue(instruction.OpcodeTable, out var redirect))
            {
                throw new InvalidOperationException($"No redirect instruction was found for opcode table {instruction.OpcodeTable}.");
            }

            return CyclesBefore(redirect) + redirect.Steps.Select((step, index) => StepCost(redirect, step, index)).Sum();
        }

        return fetchCycles * (instruction.Prefix.HasValue ? 2 : 1);
    }

    /// <summary>
    /// The cycles the paths through a sequence take from one of its steps onwards.
    /// </summary>
    [Pure]
    private IEnumerable<int> Paths(StepSequence sequence, int startIndex, IReadOnlySet<StepSequence> visited)
    {
        var total = 0;
        for (var index = startIndex; index < sequence.Steps.Count; index++)
        {
            var step = sequence.Steps[index];
            total += StepCost(sequence, step, index);

            var outcomes = new List<Outcome>();
            FindOutcomes(step.Statements, 0, null, outcomes);
            var distinct = outcomes.Distinct().ToList();
            if (distinct.All(outcome => outcome.Kind == OutcomeKind.Continue))
            {
                continue;
            }

            foreach (var outcome in distinct)
            {
                switch (outcome.Kind)
                {
                    case OutcomeKind.Complete:
                        yield return total;
                        break;

                    case OutcomeKind.Continue:
                        foreach (var rest in Paths(sequence, index + 1, visited))
                        {
                            yield return total + rest;
                        }
                        break;

                    case OutcomeKind.MoveToSequence:
                        var next = model.GetSequence(outcome.Sequence!);
                        if (visited.Contains(next))
                        {
                            throw new InvalidOperationException($"The sequence {outcome.Sequence} is reached more than once on a path, so the cycles cannot be calculated.");
                        }

                        foreach (var rest in Paths(next, 0, new HashSet<StepSequence>(visited) { next }))
                        {
                            yield return total + rest;
                        }
                        break;
                }
            }

            yield break;
        }

        yield return total;
    }

    /// <summary>
    /// Finds how each path through a step's statements leaves the step: carrying on to the next step, completing the
    /// instruction, or moving to a named sequence once the step is done.
    /// </summary>
    private static void FindOutcomes(IReadOnlyList<Statement> statements, int index, string? moveToSequence, List<Outcome> outcomes)
    {
        for (; index < statements.Count; index++)
        {
            switch (statements[index])
            {
                case CallStatement { Call.Function: var function } when function == PreDefinedFunction.InstructionComplete:
                    outcomes.Add(new Outcome(OutcomeKind.Complete, null));
                    return;

                case CallStatement { Call: { Function: var function, Arguments: [SequenceAccess sequence] } } when function == PreDefinedFunction.MoveToSequence:
                    moveToSequence = sequence.SequenceName;
                    break;

                case IfStatement ifStatement:
                    var rest = statements.Skip(index + 1).ToList();
                    FindOutcomes([.. ifStatement.IfStatements, .. rest], 0, moveToSequence, outcomes);
                    FindOutcomes([.. ifStatement.ElseStatements, .. rest], 0, moveToSequence, outcomes);
                    return;
            }
        }

        outcomes.Add(moveToSequence != null ? new Outcome(OutcomeKind.MoveToSequence, moveToSequence) : new Outcome(OutcomeKind.Continue, null));
    }

    /// <summary>
    /// The cycles a step takes. When the CPU counts <see cref="CycleCount.Steps" /> every step is a cycle except a last step
    /// that overlaps with the next opcode fetch, either because the sequence is overlapped or because the step requests the fetch
    /// itself, as the Z80's <c>EI</c> does. When it counts <see cref="CycleCount.Actions" /> only steps that request an action
    /// are cycles.
    /// </summary>
    [Pure]
    private int StepCost(StepSequence sequence, Step step, int index)
    {
        switch (cycles.Count)
        {
            case CycleCount.Actions:
                return step.SpecifiedAction != null ? 1 : 0;

            default:
                var last = index == sequence.Steps.Count - 1;
                return last && (sequence.NextOpcode == NextOpcodeMode.Overlapped || step.SpecifiedAction?.Name == "opcode_read") ? 0 : 1;
        }
    }

    private enum OutcomeKind
    {
        Continue,
        Complete,
        MoveToSequence
    }

    private sealed record Outcome(OutcomeKind Kind, string? Sequence);
}