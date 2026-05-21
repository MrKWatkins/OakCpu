using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class GeneratorContextTests : TestFixture
{
    [Test]
    public void Create()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);
        generatorInput.Cpu.Name.Should().Equal("Z80");
        generatorInput.OpcodeRead.Should().HaveCount(4);
        generatorInput.OverlapSteps.Should().NotBeEmpty();
    }

    [Test]
    public async Task CreateAsync()
    {
        var generatorInput = await GeneratorContext.CreateAsync("MrKWatkins.OakCpu.Z80", new DirectoryInfo(Z80DefinitionsDirectory));

        generatorInput.Cpu.Name.Should().Equal("Z80");
        generatorInput.Instructions.Count.Should().Equal(Z80GeneratorContext.Instructions.Count);
        generatorInput.PrefixJumps.Count.Should().Equal(Z80GeneratorContext.PrefixJumps.Count);
    }

    [Test]
    public void CreateAsync_ThrowsWhenYamlCannotBeLoaded()
    {
        var directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"oakcpu-codegen-{Guid.NewGuid():N}"));
        directory.Create();

        try
        {
            File.WriteAllText(Path.Combine(directory.FullName, "broken.yaml"), "cpu: [");

            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => _ = await GeneratorContext.CreateAsync("MrKWatkins.OakCpu.Z80", directory));

            exception!.Message.Should().Contain("Could not load YAML file");
            exception.InnerException.Should().NotBeNull();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Test]
    public void Create_DeduplicatesIdenticalOverlapHandlers()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);

        var ldcc = generatorInput.Instructions.Single(i => i.Prefix == null && i.Opcode == 0x49);
        var lddd = generatorInput.Instructions.Single(i => i.Prefix == null && i.Opcode == 0x52);

        generatorInput.GetOverlapIndex(ldcc.Steps.Single()).Should().Equal(generatorInput.GetOverlapIndex(lddd.Steps.Single()));
    }

    [Test]
    public void GetImplicitInstructionStepsCompleteStatementCount_ReturnsSuffixCount_WhenStepEndsWithOnInstructionStepsComplete()
    {
        var step = Z80GeneratorContext.Instructions
            .SelectMany(instruction => instruction.Steps)
            .First(EndsWithOnInstructionStepsComplete);

        Z80GeneratorContext.GetImplicitInstructionStepsCompleteStatementCount(step).Should().Equal(Z80GeneratorContext.OnInstructionStepsComplete.Count);
    }

    [Test]
    public void GetImplicitInstructionStepsCompleteStatementCount_ReturnsZero_WhenStepDoesNotEndWithOnInstructionStepsComplete()
    {
        var step = Z80GeneratorContext.Instructions
            .SelectMany(instruction => instruction.Steps)
            .First(step => !EndsWithOnInstructionStepsComplete(step));

        Z80GeneratorContext.GetImplicitInstructionStepsCompleteStatementCount(step).Should().Equal(0);
    }

    [Test]
    public void InstructionEmulatorSequences_IncludeOpcodeReadAndPrefixJumps()
    {
        Z80GeneratorContext.InstructionEmulatorSequences.Should().Contain(Z80GeneratorContext.OpcodeRead);
        foreach (var prefixJump in Z80GeneratorContext.PrefixJumps.Values)
        {
            Z80GeneratorContext.InstructionEmulatorSequences.Should().Contain(prefixJump);
        }
    }

    [Test]
    public void Create_FinalizesStepLayouts()
    {
        var context = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);

        AssertThat.Invoking(() =>
        {
            foreach (var step in context.AllSteps)
            {
                _ = context.GetStepLayout(step).Sequence;
                _ = context.GetStepLayout(step).Index;
                _ = context.GetStepLayout(step).MethodIndex;
                _ = context.GetStepLayout(step).Implementation;
            }
        }).Should().NotThrow();
    }

    [Test]
    public void CreateFileContext()
    {
        var newContext = Z80GeneratorContext.CreateFileContext();

        newContext.RequiredUsings.Count.Should().Equal(0);
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
    }

    [Test]
    public void Create_ThrowsGroupedValidationErrors()
    {
        var yaml = YamlSerializer.Deserialize<YamlFile>(
            """
                cpu:
                  name: TestCpu
                  actions:
                    - name: opcode_read
                      documentation: Reads an opcode.
                interrupts:
                  modes:
                    - number: 0
                      sequence: missing_interrupt_sequence
                instructions:
                  - group: test
                    mnemonic: FIRST
                    opcodes:
                      - opcode: 0x00
                      - opcode: 0x00
                    next_opcode: custom
                    steps:
                      - request(action.opcode_read);
                  - group: test
                    mnemonic: SECOND
                    opcodes:
                      - opcode: 0x01
                    next_opcode: custom
                    overlapped_sequence: missing_overlap_sequence
                    steps:
                      - request(action.opcode_read);
                """u8.ToArray(),
            YamlOptions.Instance);

        var exception = Assert.Throws<InvalidOperationException>(() => _ = GeneratorContext.Create("MrKWatkins.OakCpu.TestCpu", yaml));

        Assert.That(exception!.Message, Does.Contain("Definition validation failed:"));
        Assert.That(exception.Message, Does.Contain("No opcode_read sequence has been defined."));
        Assert.That(exception.Message, Does.Contain("interrupts.modes[0].sequence: No sequence named missing_interrupt_sequence exists for interrupt mode 0."));
        Assert.That(exception.Message, Does.Contain("instructions[1].next_opcode: Instruction SECOND specifies an overlapped sequence but does not use next_opcode: overlapped."));
        Assert.That(exception.Message, Does.Contain("instructions[1].overlapped_sequence: Instruction SECOND references unknown overlapped sequence missing_overlap_sequence."));
        Assert.That(exception.Message, Does.Contain("instructions[0].opcodes[0].opcode, instructions[0].opcodes[1].opcode: The opcodes are defined multiple times by instruction FIRST: 0x00"));
    }

    [Pure]
    private static bool EndsWithOnInstructionStepsComplete(Step step)
    {
        var suffix = Z80GeneratorContext.OnInstructionStepsComplete;
        return suffix.Count != 0 &&
               step.Statements.Count >= suffix.Count &&
               step.Statements
                   .Skip(step.Statements.Count - suffix.Count)
                   .Zip(suffix, ReferenceEquals)
                   .All(match => match);
    }
}