using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator;

public sealed class GeneratorInput
{
    private GeneratorInput(string rootNamespace, Cpu cpu, IReadOnlyList<Register> registers, Dictionary<string, Flag> flags, IReadOnlyList<Instruction> instructions, Dictionary<string, UserDefinedFunction> userDefinedFunctions)
    {
        VerifyNoDuplicateOpcodes(instructions);
        RootNamespace = rootNamespace;
        Cpu = cpu;
        Registers = registers;
        Flags = flags;
        Instructions = instructions;
        UserDefinedFunctions = userDefinedFunctions;
        FlagsRegister = Registers.Single(r => r.Flags);
        ProgramCounter = Registers.Single(r => r.ProgramCounter);
        Step.AssignIndexes(AllSteps);
    }

    public string RootNamespace { get; }

    public Cpu Cpu { get; }

    public IReadOnlyList<Register> Registers { get; }

    public IReadOnlyDictionary<string, Flag> Flags { get; }

    public IReadOnlyList<Instruction> Instructions { get; }

    public IReadOnlyDictionary<string, UserDefinedFunction> UserDefinedFunctions { get; }

    public Register FlagsRegister { get; }

    public Register ProgramCounter { get; }

    public IEnumerable<Step> AllSteps => Cpu.OpcodeRead.Concat(Instructions.SelectMany(i => i.Steps));

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

        var registers = Register.Create(yaml.Registers);
        var registersByName = registers.ToDictionary(r => r.Name);

        var flags = Flag.Create(yaml.Flags).ToDictionary(f => f.Name);

        var cpu = Cpu.Create(registersByName, yaml.Cpu);

        var context = new ParserContext(new HashSet<string>(cpu.Actions), registersByName);
        var userDefinedFunctions = UserDefinedFunction.Create(context, yaml.Functions).ToDictionary(f => f.Name);

        context = context.WithFunctions(userDefinedFunctions);

        return new GeneratorInput(rootNamespace, cpu, registers, flags, Instruction.Create(context, yaml.Instructions), userDefinedFunctions);
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
        var duplicates = instructions.GroupBy(i => i.Opcode).Where(g => g.Count() > 1).ToList();
        if (duplicates.Count > 0)
        {
            throw new InvalidOperationException(
                $"The following opcodes are defined multiple times: {string.Join("\n", duplicates.Select(g => $"0x{g.Key:X2} [{string.Join(", ", g.Select(i => i.Mnemonic))}]"))}");
        }
    }
}