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
    private GeneratorInput(string rootNamespace, IReadOnlyList<Register> registers, IReadOnlyList<Flag> flags, IReadOnlyList<Instruction> instructions)
    {
        RootNamespace = rootNamespace;
        Registers = registers;
        Flags = flags;
        Instructions = instructions;
        FlagsRegister = Registers.Single(r => r.Flags);
        ProgramCounter = Registers.Single(r => r.ProgramCounter);
    }

    public string RootNamespace { get; }

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

        return new GeneratorInput(rootNamespace, Register.Create(yaml.Registers), Flag.Create(yaml.Flags), Instruction.Create(yaml.Instructions));
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