using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator;

public sealed class GeneratorInput
{
    private GeneratorInput(string rootNamespace, Configuration configuration, Cpu cpu, Interrupts interrupts, IReadOnlyList<Step> opcodeRead, IReadOnlyList<Instruction> instructions)
    {
        VerifyNoDuplicateOpcodes(instructions);
        RootNamespace = rootNamespace;
        Configuration = configuration;
        Cpu = cpu;
        Interrupts = interrupts;
        OpcodeRead = opcodeRead;
        Instructions = instructions;
        OpcodePrefixes = Instructions.Where(i => i.Prefix.HasValue).Select(i => i.Prefix!.Value).Distinct().OrderBy(p => p).ToList();
        Step.AssignIndexes(AllSteps);
    }

    public string RootNamespace { get; }

    public Configuration Configuration { get; }

    public Cpu Cpu { get; }

    public Interrupts Interrupts { get; }

    public IReadOnlyList<Step> OpcodeRead { get; }

    public IReadOnlyList<Instruction> Instructions { get; }

    public IReadOnlyList<byte> OpcodePrefixes { get; }

    // Steps need to keep their order within an instruction or all hell breaks loose.
    public IEnumerable<Step> AllSteps => OpcodeRead.Concat(Instructions.SelectMany(i => i.Steps));

    [Pure]
    public NamespaceDeclarationSyntax CreateRootNamespaceDeclaration() => SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(RootNamespace));

    [Pure]
    public static GeneratorInput Create(string? rootNamespace, ImmutableArray<AdditionalText> yamls) =>
        Create(rootNamespace, YamlFile.Combine(yamls.Select(LoadYaml)));

    [Pure]
    internal static GeneratorInput Create(string? rootNamespace, YamlFile yaml)
    {
        if (rootNamespace == null)
        {
            throw new ArgumentNullException(nameof(rootNamespace));
        }

        var actions = Action.Create(yaml.Cpu);
        var registers = Register.Create(yaml.Registers);
        var flags = Flag.Create(yaml.Flags);
        var opcodeStepTables = new OpcodeStepTables(yaml.Instructions);
        var userDefinedDataMembers = UserDefinedDataMember.Create(yaml.Cpu.Fields, DataMemberVisibility.Private)
            .Concat(UserDefinedDataMember.Create(yaml.Interrupts.Properties, DataMemberVisibility.Internal))
            .ToDictionary(u => u.Name);

        var configuration = new Configuration(actions, registers, flags, opcodeStepTables, userDefinedDataMembers);

        UserDefinedFunction.AddToConfiguration(configuration, yaml.Functions);

        var context = new ParserContext(configuration);

        var opcodeRead = Step.Parse("Opcode read", context, yaml.OpcodeRead);

        var instructions = Instruction.Create(context, yaml.Instructions);

        return new GeneratorInput(rootNamespace, configuration, Cpu.Create(yaml.Cpu), Interrupts.Create(yaml.Interrupts), opcodeRead, instructions);
    }

    [Pure]
    private static YamlFile LoadYaml(AdditionalText text)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(text.GetText()!.ToString());
            return YamlSerializer.Deserialize<YamlFile>(bytes, YamlOptions.Instance);
        }
        catch (Exception exception)
        {
            throw new IOException($"Could not load YAML file {Path.GetFullPath(text.Path)}.", exception);
        }
    }

    private static void VerifyNoDuplicateOpcodes(IReadOnlyList<Instruction> instructions)
    {
        foreach (var group in instructions.GroupBy(i => i.OpcodeTable))
        {
            var duplicates = group.GroupBy(i => (i.Prefix, i.Opcode)).Where(g => g.Count() > 1).ToList();
            if (duplicates.Count > 0)
            {
                var groupText = group != null ? $"in opcode table {group.Key}" : "";

                throw new InvalidOperationException(
                    $"The following opcodes {groupText} are defined multiple times: {string.Join("\n", duplicates.Select(g => $"{(g.Key.Prefix.HasValue ? $"0x{g.Key.Prefix.Value:X2} " : "")}0x{g.Key.Opcode:X2} [{string.Join(", ", g.Select(i => i.Mnemonic))}]"))}");
            }
        }
    }
}