using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator;

public sealed class GeneratorInput
{
    private GeneratorInput(string rootNamespace, Cpu cpu, IReadOnlyList<Register> registers, IReadOnlyList<Flag> flags, IReadOnlyList<Instruction> instructions)
    {
        RootNamespace = rootNamespace;
        Cpu = cpu;
        Registers = registers;
        Flags = flags;
        Instructions = instructions;
        FlagsRegister = Registers.Single(r => r.Flags);
        ProgramCounter = Registers.Single(r => r.ProgramCounter);
    }

    public string RootNamespace { get; }

    public Cpu Cpu { get; }

    public IReadOnlyList<Register> Registers { get; }

    public IReadOnlyList<Flag> Flags { get; }

    public IReadOnlyList<Instruction> Instructions { get; }

    public Register FlagsRegister { get; }

    public Register ProgramCounter { get; }

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

        var flags = Flag.Create(yaml.Flags);
        var flagsByName = flags.ToDictionary(r => r.Name);

        var cpu = Cpu.Create(yaml.Cpu, registersByName, flagsByName);

        return new GeneratorInput(rootNamespace, cpu, registers, flags, Instruction.Create(yaml.Instructions));
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
}