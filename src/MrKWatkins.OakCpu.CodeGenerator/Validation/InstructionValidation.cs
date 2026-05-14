using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal static class InstructionValidation
{
    [Pure]
    public static IEnumerable<ValidationError> Validate(IReadOnlyList<InstructionYaml> instructions, YamlFile yaml)
    {
        var availableSequenceNames = ValidationHelpers.GetAvailableSequenceNames(yaml);

        return ValidateDuplicateOpcodes(instructions)
            .Concat(ValidateOverlappedSequences(instructions, availableSequenceNames));
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateDuplicateOpcodes(IReadOnlyList<InstructionYaml> instructions)
    {
        foreach (var duplicate in instructions
                     .Indexed()
                     .SelectMany(item => item.Item.Opcodes.Indexed().Select(
                         opcode => (
                             item.Item.OpcodeTable,
                             opcode.Item.PrefixByte,
                             opcode.Item.OpcodeByte,
                             Instruction: item.Item,
                             Path: $"instructions[{item.Index}].opcodes[{opcode.Index}].opcode")))
                     .GroupBy(opcode => (opcode.OpcodeTable, opcode.PrefixByte, opcode.OpcodeByte))
                     .Where(group => group.Count() > 1)
                     .OrderBy(group => group.Key.OpcodeTable, StringComparer.Ordinal)
                     .ThenBy(group => group.Key.PrefixByte)
                     .ThenBy(group => group.Key.OpcodeByte))
        {
            var groupText = duplicate.Key.OpcodeTable != null ? $"in opcode table {duplicate.Key.OpcodeTable} " : "";
            var mnemonics = ValidationHelpers.FormatNames(duplicate.Select(item => item.Instruction.Mnemonic));
            var instructionText = duplicate.Select(item => item.Instruction.Mnemonic).Distinct(StringComparer.Ordinal).Count() == 1 ? "instruction" : "instructions";
            yield return new ValidationError(
                $"The opcodes {groupText}are defined multiple times by {instructionText} {mnemonics}: {(duplicate.Key.PrefixByte.HasValue ? $"0x{duplicate.Key.PrefixByte.Value:X2} " : "")}0x{duplicate.Key.OpcodeByte:X2}",
                duplicate.Select(item => item.Path).OrderBy(path => path, StringComparer.Ordinal).ToArray());
        }
    }

    [Pure]
    private static IEnumerable<ValidationError> ValidateOverlappedSequences(IReadOnlyList<InstructionYaml> instructions, HashSet<string> availableSequenceNames)
    {
        foreach (var (instruction, index) in instructions.Indexed()
                     .Where(item => item.Item.OverlappedSequence != null)
                     .Select(item => (item.Item, item.Index)))
        {
            if (instruction.NextOpcode != NextOpcodeMode.Overlapped)
            {
                yield return new ValidationError(
                    $"Instruction {instruction.Mnemonic} specifies an overlapped sequence but does not use next_opcode: overlapped.",
                    $"instructions[{index}].next_opcode");
            }

            if (instruction.OverlappedSequence != null && !availableSequenceNames.Contains(instruction.OverlappedSequence))
            {
                yield return new ValidationError(
                    $"Instruction {instruction.Mnemonic} references unknown overlapped sequence {instruction.OverlappedSequence}.",
                    $"instructions[{index}].overlapped_sequence");
            }
        }
    }
}