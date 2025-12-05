using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class GeneratorContext
{
    private GeneratorContext(string rootNamespace, Configuration configuration, Cpu cpu, Interrupts interrupts, OpcodeRead opcodeRead, IReadOnlyList<Statement> onInstructionComplete, IReadOnlyList<Instruction> instructions, IReadOnlyDictionary<byte, PrefixJump> prefixJumps, IReadOnlyList<Step> allSteps)
    {
        RootNamespace = rootNamespace;
        Configuration = configuration;
        Cpu = cpu;
        Interrupts = interrupts;
        OpcodeRead = opcodeRead;
        OnInstructionComplete = onInstructionComplete;
        Instructions = instructions;
        PrefixJumps = prefixJumps;
        AllSteps = allSteps;
    }

    public string RootNamespace { get; }

    public Configuration Configuration { get; }

    public Cpu Cpu { get; }

    public Interrupts Interrupts { get; }

    public OpcodeRead OpcodeRead { get; }

    public IReadOnlyList<Statement> OnInstructionComplete { get; }

    public IReadOnlyList<Instruction> Instructions { get; }

    public IReadOnlyDictionary<byte, PrefixJump> PrefixJumps { get; }

    public IReadOnlyList<Step> AllSteps { get; }

    public HashSet<string> RequiredUsings { get; } = new();

    public int ErrorStepIndex => AllSteps.Count;

    [Pure]
    public NamespaceDeclarationSyntax CreateRootNamespaceDeclaration() => SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(RootNamespace));

    [Pure]
    public static async Task<GeneratorContext> CreateAsync(string rootNamespace, DirectoryInfo definitionsDirectory)
    {
        var yamls = new List<YamlFile>();
        foreach (var file in definitionsDirectory.GetFiles("*.yaml", SearchOption.AllDirectories))
        {
            yamls.Add(await DeserializeYamlAsync<YamlFile>(file));
        }

        var combined = YamlFile.Combine(yamls);

        return Create(rootNamespace, combined);
    }

    [Pure]
    private static async Task<TYaml> DeserializeYamlAsync<TYaml>(FileInfo file)
    {
        try
        {
            await using var stream = file.OpenRead();
            return await YamlSerializer.DeserializeAsync<TYaml>(stream, YamlOptions.Instance);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Could not load YAML file {file}.", exception);
        }
    }

    [Pure]
    internal static GeneratorContext Create(string rootNamespace, YamlFile yaml)
    {
        var actions = Action.Create(yaml.Cpu);
        var registers = Register.Create(yaml.Registers);
        var flags = Flag.Create(yaml.Flags);
        var opcodeStepTables = new OpcodeStepTables(yaml.Instructions);
        var userDefinedDataMembers = UserDefinedDataMember.Create(yaml.Cpu.Fields, Visibility.Private)
            .Concat(UserDefinedDataMember.Create(yaml.Interrupts.Properties, Visibility.Internal))
            .ToDictionary(u => u.Name);

        var configuration = new Configuration(actions, registers, flags, opcodeStepTables, userDefinedDataMembers);

        UserDefinedFunction.AddToConfiguration(configuration, yaml.Functions);

        var context = new ParserContext(configuration);

        context = context.WithOnInstructionComplete(Parser.ParseStatements(context, yaml.OnInstructionComplete));

        var opcodeRead = OpcodeRead.Create(context, yaml);

        var instructions = Instruction.Create(context, yaml.Instructions);

        var prefixJumps = PrefixJump.Create(context, instructions);

        var interrupts = Interrupts.Create(context, yaml.Interrupts);

        // Steps need to keep their order within an instruction or all hell breaks loose.
        var allSteps = opcodeRead
            .Concat(prefixJumps.Values.SelectMany(p => p.Steps))
            .Concat(interrupts.AllSteps)
            .Concat(instructions.SelectMany(i => i.Steps)).ToList();
        Step.AssignIndexes(allSteps);

        return new GeneratorContext(rootNamespace, configuration, Cpu.Create(yaml.Cpu), interrupts, opcodeRead, context.OnInstructionComplete, instructions, prefixJumps, allSteps);
    }

    [Pure]
    internal GeneratorContext WithRequiredUsings() => new(RootNamespace, Configuration, Cpu, Interrupts, OpcodeRead, OnInstructionComplete, Instructions, PrefixJumps, AllSteps);
}