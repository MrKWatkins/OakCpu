using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class SequenceValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(YamlFile yaml)
    {
        var availableSequenceNames = ValidationHelpers.GetAvailableSequenceNames(yaml);

        return ValidationHelpers.ValidateDuplicateNames(
                yaml.Sequences.Indexed().Select(item => (item.Item.Name, $"sequences[{item.Index}].name")),
                "sequence")
            .Concat(ValidateOpcodeRead(yaml))
            .Concat(ValidateHaltedSequence(yaml, availableSequenceNames))
            .Concat(ValidateSequenceGroups(yaml.Sequences))
            .Concat(ValidateInterruptModes(yaml, availableSequenceNames));
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateOpcodeRead(YamlFile yaml)
    {
        if (!yaml.OpcodeRead.Any() && !yaml.Sequences.Any(sequence => sequence.Name == "opcode_read"))
        {
            yield return new ValidationError("No opcode_read sequence has been defined.");
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateHaltedSequence(YamlFile yaml, HashSet<string> availableSequenceNames)
    {
        if (!yaml.Interrupts.HaltedCycle.Any() && !availableSequenceNames.Contains("halted") && !availableSequenceNames.Contains("halted_cycle"))
        {
            yield return new ValidationError("No sequence named halted exists for the halted cycle.");
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateSequenceGroups(IReadOnlyList<StepSequenceYaml> sequences)
    {
        foreach (var duplicate in sequences
                     .Indexed()
                     .Where(item => item.Item.Group != null)
                     .GroupBy(item => (item.Item.Group!.Name, item.Item.Group.Number))
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key.Name, StringComparer.Ordinal)
                     .ThenBy(group => group.Key.Number))
        {
            var sequenceNames = ValidationHelpers.FormatNames(duplicate.Select(item => item.Item.Name));
            yield return new ValidationError(
                $"The sequence group {duplicate.Key.Name} contains multiple sequences for number {duplicate.Key.Number}: {sequenceNames}.",
                duplicate.Select(item => $"sequences[{item.Index}].group.number").OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateInterruptModes(YamlFile yaml, HashSet<string> availableSequenceNames)
    {
        var explicitInterruptModes = yaml.Sequences
            .Where(sequence => sequence.Group?.Name == InterruptMode.SequenceGroupName)
            .Select(sequence => sequence.Group!.Number)
            .ToHashSet();

        foreach (var duplicate in yaml.Interrupts.Modes
                     .Indexed()
                     .Where(item => !explicitInterruptModes.Contains(item.Item.Number))
                     .GroupBy(item => item.Item.Number)
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key))
        {
            yield return new ValidationError(
                $"Interrupt mode {duplicate.Key} is defined multiple times.",
                duplicate.Select(item => $"interrupts.modes[{item.Index}].number").OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }

        foreach (var (mode, index) in yaml.Interrupts.Modes.Indexed()
                     .Where(item => item.Item.Sequence != null && !availableSequenceNames.Contains(item.Item.Sequence))
                     .Select(item => (item.Item, item.Index)))
        {
            yield return new ValidationError(
                $"No sequence named {mode.Sequence} exists for interrupt mode {mode.Number}.",
                $"interrupts.modes[{index}].sequence");
        }
    }
}