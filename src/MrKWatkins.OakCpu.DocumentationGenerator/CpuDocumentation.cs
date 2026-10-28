using System.Globalization;
using System.Text.RegularExpressions;
using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.DocumentationGenerator;

/// <summary>
/// Builds the documentation for every instruction of a CPU from its model.
/// </summary>
public static partial class CpuDocumentation
{
    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex SlugSeparatorRegex();

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_']*|0x[0-9A-Fa-f]+|[0-9]+")]
    private static partial Regex TokenRegex();

    /// <summary>
    /// Builds the documentation for every instruction of the CPU, ordered by group then mnemonic. Instructions that only redirect
    /// to another opcode table are not documented.
    /// </summary>
    [Pure]
    public static IReadOnlyList<InstructionDocumentation> Create(CpuModel model)
    {
        var instructionsByOpcode = model.Instructions
            .SelectMany(instruction => instruction.Duplicates
                .Select(duplicate => (Key: (instruction.OpcodeTable, duplicate.Prefix, duplicate.Opcode), Instruction: instruction))
                .Prepend((Key: (instruction.OpcodeTable, instruction.Prefix, instruction.Opcode), Instruction: instruction)))
            .ToDictionary(x => x.Key, x => x.Instruction);

        var tStates = new TStateCalculator(model);
        var encoding = new OpcodeEncoding(model);

        var documentation = model.Yaml.Instructions
            .Where(yaml => !TStateCalculator.IsRedirect(instructionsByOpcode[Key(yaml, yaml.Opcodes[0])]))
            .GroupBy(yaml => yaml.Mnemonic, StringComparer.Ordinal)
            .Select(form => Create(model, form.Key, form.ToList(), instructionsByOpcode, tStates, encoding))
            .OrderBy(d => d.Group, StringComparer.Ordinal)
            .ThenBy(d => d.Form, StringComparer.Ordinal)
            .ToList();

        var duplicatePaths = documentation.GroupBy(d => d.Path, StringComparer.Ordinal).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicatePaths.Count > 0)
        {
            throw new InvalidOperationException($"Multiple instructions have the same documentation path: {string.Join(", ", duplicatePaths)}.");
        }

        return documentation;
    }

    [Pure]
    private static InstructionDocumentation Create(
        CpuModel model,
        string form,
        IReadOnlyList<InstructionYaml> entries,
        IReadOnlyDictionary<(string? Table, byte? Prefix, byte Opcode), Instruction> instructionsByOpcode,
        TStateCalculator tStates,
        OpcodeEncoding encoding)
    {
        // A form can have several entries when some opcodes behave differently internally, e.g. the Z80's prefixed SCF and
        // CCF. The entry with an unprefixed opcode is the canonical one.
        var canonicalEntry = entries.FirstOrDefault(entry => entry.Opcodes.Any(opcode => opcode.PrefixByte == null)) ?? entries[0];

        var descriptions = entries.Select(entry => entry.Documentation).Where(d => !string.IsNullOrWhiteSpace(d)).Distinct(StringComparer.Ordinal).ToList();
        var description = descriptions.Count switch
        {
            0 => throw new InvalidOperationException($"The instruction {form} has no documentation."),
            1 => descriptions[0]!.Trim(),
            _ => throw new InvalidOperationException($"The entries for the instruction {form} have different documentation.")
        };

        var placeholders = Operands.PlaceholdersIn(form);
        var unknownPlaceholders = Operands.PlaceholdersInDocumentation(description).Except(placeholders, StringComparer.Ordinal).ToList();
        if (unknownPlaceholders.Count > 0)
        {
            throw new InvalidOperationException($"The documentation for the instruction {form} uses placeholders that are not in its mnemonic: {string.Join(", ", unknownPlaceholders)}.");
        }

        var opcodes = entries
            .SelectMany(entry => entry.Opcodes.Select(opcode => (Entry: entry, Opcode: opcode)))
            .OrderBy(x => x.Entry.OpcodeTable, StringComparer.Ordinal)
            .ThenBy(x => x.Opcode, OpcodeYamlNoPrefixFirstComparer.Instance)
            .ToList();

        var undocumented = opcodes.Select(x => IsUndocumented(model.Configuration, x.Entry, x.Opcode, opcodes)).ToList();

        // Bit numbers are shown in decimal; anything larger, i.e. the page zero addresses of RST, in hex.
        var hexNumbers = opcodes.Any(x => x.Opcode.N0 > 7);

        var opcodeDocumentation = opcodes
            .Select((x, index) =>
            {
                var instruction = instructionsByOpcode[Key(x.Entry, x.Opcode)];
                var operands = Operands.For(form, x.Opcode, hexNumbers);
                return new OpcodeDocumentation(
                    encoding.Format(form, instruction, x.Opcode.PrefixByte, x.Opcode.OpcodeByte),
                    Operands.SubstituteMnemonic(form, operands),
                    operands,
                    tStates.Calculate(instruction),
                    undocumented[index]);
            })
            .ToList();

        var canonicalOpcode = ChooseCanonicalOpcode(canonicalEntry);
        var canonicalInstruction = instructionsByOpcode[Key(canonicalEntry, canonicalOpcode)];
        var operandPlaceholders = Operands.For(form, canonicalOpcode, hexNumbers)
            .Where(operand => operand.Key != "N0")
            .DistinctBy(operand => operand.Value, StringComparer.Ordinal)
            .ToDictionary(operand => operand.Value, operand => $"{{{operand.Key}}}", StringComparer.Ordinal);
        var describer = new FlagEffectDescriber(model.Configuration, canonicalInstruction, operandPlaceholders);
        var flags = ApplyFlagDocumentation(form, describer.Describe(), entries, model.Configuration);

        // Flags documented by hand do not need their expressions describing.
        var documentedFlags = entries.SelectMany(entry => entry.FlagDocumentation.Keys).ToHashSet(StringComparer.Ordinal);
        var fallbacks = describer.Fallbacks
            .Where(fallback => !documentedFlags.Contains(fallback.Key))
            .Select(fallback => $"{fallback.Key}: {fallback.Value}")
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var definedBy = model.Yaml.InheritedFrom.GetValueOrDefault(canonicalEntry) ?? model.Cpu.Name;
        var allUndocumented = undocumented.All(u => u);

        return new InstructionDocumentation(
            model.Cpu.Name,
            definedBy,
            form,
            Operands.SubstituteMnemonic(form, Operands.Symbolic(form, opcodes.Select(x => x.Opcode).ToList())),
            canonicalEntry.Group,
            description,
            allUndocumented,
            flags,
            opcodeDocumentation,
            References.For(definedBy, allUndocumented),
            $"{Slug(canonicalEntry.Group)}/{Slug(form)}.md",
            fallbacks);
    }

    /// <summary>
    /// Replaces the automatic descriptions of flag effects with any written in the definitions.
    /// </summary>
    [Pure]
    private static IReadOnlyList<FlagEffect> ApplyFlagDocumentation(string form, IReadOnlyList<FlagEffect> flags, IReadOnlyList<InstructionYaml> entries, Configuration configuration)
    {
        var overrides = entries
            .SelectMany(entry => entry.FlagDocumentation)
            .GroupBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(kvp => kvp.Value).Distinct(StringComparer.Ordinal).ToList(), StringComparer.Ordinal);

        foreach (var (flag, descriptions) in overrides)
        {
            if (!configuration.Flags.ContainsKey(flag))
            {
                throw new InvalidOperationException($"The instruction {form} documents the flag {flag}, which does not exist.");
            }

            if (descriptions.Count > 1)
            {
                throw new InvalidOperationException($"The entries for the instruction {form} have different documentation for the flag {flag}.");
            }
        }

        return flags
            .Select(effect => overrides.TryGetValue(effect.Flag.Name, out var descriptions)
                ? effect with { Kind = effect.Kind == FlagEffectKind.Unaffected ? FlagEffectKind.Changed : effect.Kind, Description = descriptions[0].Trim() }
                : effect)
            .ToList();
    }

    [Pure]
    private static (string? Table, byte? Prefix, byte Opcode) Key(InstructionYaml entry, OpcodeYaml opcode) => (entry.OpcodeTable, opcode.PrefixByte, opcode.OpcodeByte);

    /// <summary>
    /// Chooses the opcode whose flag expressions are described for the whole instruction. Operands are turned back into
    /// placeholders in the descriptions, so prefer an opcode whose operands do not also appear literally in the instruction's
    /// definition, e.g. for <c>ADD A, R0</c> prefer an opcode where <c>R0</c> is not <c>A</c>.
    /// </summary>
    [Pure]
    private static OpcodeYaml ChooseCanonicalOpcode(InstructionYaml entry)
    {
        var literals = entry.Steps
            .Concat(entry.Flags.Values)
            .Where(text => text != null)
            .SelectMany(text => TokenRegex().Matches(text!).Select(match => match.Value))
            .ToHashSet(StringComparer.Ordinal);

        var ordered = entry.Opcodes.Order(OpcodeYamlNoPrefixFirstComparer.Instance).ToList();

        return ordered.FirstOrDefault(opcode => HasDistinctOperands(opcode) && Operands.Placeholders.All(placeholder => Operands.Get(opcode, placeholder) is not { } value || !literals.Contains(value)))
               ?? ordered.FirstOrDefault(HasDistinctOperands)
               ?? ordered[0];
    }

    /// <summary>
    /// Whether an opcode is undocumented. An opcode is undocumented if its instruction or the opcode itself is marked as such,
    /// if it uses an undocumented register, or if it duplicates a documented opcode of the same instruction with the same
    /// operands, e.g. the Z80's <c>DD</c> and <c>FD</c> prefixed copies of unprefixed instructions.
    /// </summary>
    [Pure]
    private static bool IsUndocumented(Configuration configuration, InstructionYaml entry, OpcodeYaml opcode, IReadOnlyList<(InstructionYaml Entry, OpcodeYaml Opcode)> all)
    {
        if (IsMarkedUndocumented(configuration, entry, opcode))
        {
            return true;
        }

        var canonical = all
            .Where(x => !IsMarkedUndocumented(configuration, x.Entry, x.Opcode) && HasSameOperands(x.Opcode, opcode))
            .Select(x => x.Opcode)
            .Order(OpcodeYamlNoPrefixFirstComparer.Instance)
            .First();

        return !ReferenceEquals(canonical, opcode);
    }

    [Pure]
    private static bool IsMarkedUndocumented(Configuration configuration, InstructionYaml entry, OpcodeYaml opcode) =>
        entry.Undocumented ||
        opcode.Undocumented ||
        new[] { opcode.R0, opcode.R1, opcode.RP0, opcode.RP1 }.Any(register => register != null && configuration.Registers[register].Undocumented);

    /// <summary>
    /// Whether every operand of an opcode is different, e.g. <c>LD B, C</c> rather than <c>LD B, B</c>, so that each operand
    /// can be turned back into its placeholder.
    /// </summary>
    [Pure]
    private static bool HasDistinctOperands(OpcodeYaml opcode)
    {
        var values = Operands.Placeholders.Select(placeholder => Operands.Get(opcode, placeholder)).Where(value => value != null).ToList();
        return values.Distinct(StringComparer.Ordinal).Count() == values.Count;
    }

    [Pure]
    private static bool HasSameOperands(OpcodeYaml x, OpcodeYaml y) =>
        x.R0 == y.R0 && x.R1 == y.R1 && x.RP0 == y.RP0 && x.RP1 == y.RP1 && x.C0 == y.C0 && x.N0 == y.N0;

    /// <summary>
    /// Makes a file name from text, e.g. <c>ld-r0-at-rp0-d</c> from <c>LD R0, (RP0 + d)</c>. Indirection is kept as <c>at</c> so
    /// that <c>LD RP0, (nn)</c> and <c>LD RP0, nn</c> get different names, and shadow registers as <c>shadow</c>.
    /// </summary>
    [Pure]
    public static string Slug(string text) =>
        SlugSeparatorRegex()
            .Replace(
                text.ToLower(CultureInfo.InvariantCulture)
                    .Replace("(", " at ", StringComparison.Ordinal)
                    .Replace("'", " shadow ", StringComparison.Ordinal),
                "-")
            .Trim('-');
}