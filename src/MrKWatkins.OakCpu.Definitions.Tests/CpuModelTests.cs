using MrKWatkins.OakCpu.Definitions.Yaml;

namespace MrKWatkins.OakCpu.Definitions.Tests;

public sealed class CpuModelTests : TestFixture
{
    [Test]
    public void Create()
    {
        var model = CpuModel.Create(Z80Yaml);

        model.Cpu.Name.Should().Equal("Z80");
        model.Cpu.DisplayName.Should().Equal("Z80");
        model.Cpu.Cycles.Should().Equal(new Cycles("T-States", CycleCount.Steps, 1));
        model.Cpu.Cycles.Key.Should().Equal("t_states");
        model.Cpu.Operands.Select(operand => $"{operand.Name}={operand.Bytes}").Should().SequenceEqual("n=1", "nn=2", "d=1");
        model.Cpu.References.Select(reference => reference.Covers).Should().SequenceEqual(ReferenceCoverage.Documented, ReferenceCoverage.All, ReferenceCoverage.Undocumented);
        model.Cpu.Temporaries["result"].Should().Equal("the result");
        model.Cpu.Base.Should().BeNull();
        model.Cpu.GetCpu("Z80").Should().BeTheSameInstanceAs(model.Cpu);
        model.Configuration.UserDefinedFunctions["is_negative"].Documentation.Should().Equal("bit 7 of {value} is set");
        model.Configuration.UserDefinedFunctions["is_negative"].NotDocumentation.Should().Equal("bit 7 of {value} is reset");
        model.Configuration.UserDefinedFunctions["did_8_bit_addition_half_carry"].ValueDocumentation.Should().Equal("the half carry");
        model.OpcodeRead.Should().HaveCount(4);
        model.Instructions.Should().NotBeEmpty();
        model.PrefixJumps.Keys.Should().SequenceEqual(0xCB, 0xDD, 0xED, 0xFD);
    }

    [Test]
    public async Task LoadAsync()
    {
        var model = await CpuModel.LoadAsync(new DirectoryInfo(Z80DefinitionsDirectory));

        model.Cpu.Name.Should().Equal("Z80");
        model.Instructions.Count.Should().Equal(Z80Model.Instructions.Count);
        model.PrefixJumps.Count.Should().Equal(Z80Model.PrefixJumps.Count);
    }

    [Test]
    public async Task LoadAsync_MergesBaseCpu()
    {
        var model = await CpuModel.LoadAsync(new DirectoryInfo(GetDefinitionsDirectory("z80n")));

        model.Cpu.Name.Should().Equal("Z80N");
        model.Cpu.DisplayName.Should().Equal("Z80N");
        model.Cpu.Cycles.Should().Equal(Z80Model.Cpu.Cycles);
        model.Cpu.Operands.Select(operand => operand.Name).Should().SequenceEqual("n", "nn", "d");
        model.Cpu.References.Should().HaveCount(1);
        model.Cpu.Base!.Name.Should().Equal("Z80");
        model.Cpu.GetCpu("Z80").References.Should().HaveCount(3);
        AssertThat.Invoking(() => model.Cpu.GetCpu("6502")).Should().Throw<InvalidOperationException>();
        model.Instructions.Count.Should().Equal(Z80NModel.Instructions.Count);
        model.Instructions.Any(i => i.Mnemonic == "LDIX").Should().BeTrue();
        model.Instructions.Any(i => i.Mnemonic == "NOP").Should().BeTrue();
    }

    [Test]
    public async Task LoadAsync_ThrowsWhenYamlCannotBeLoaded()
    {
        var directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"oakcpu-definitions-{Guid.NewGuid():N}"));
        directory.Create();

        try
        {
            await File.WriteAllTextAsync(Path.Combine(directory.FullName, "broken.yaml"), "cpu: [");

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => _ = await CpuModel.LoadAsync(directory));

            exception!.Message.Should().Contain("Could not load YAML file");
            exception.InnerException.Should().NotBeNull();
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Test]
    public void GetStepLayout()
    {
        var step = Z80Model.Instructions[0].Steps[0];

        Z80Model.GetStepLayout(step).Step.Should().BeTheSameInstanceAs(step);
    }

    [Test]
    public void GetStepLayout_ThrowsForUnknownStep()
    {
        var step = Z80NModel.Instructions.Single(i => i.Mnemonic == "LDIX").Steps[0];

        Z80Model.Invoking(m => m.GetStepLayout(step)).Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void GetSequence()
    {
        Z80Model.GetSequence("opcode_read").Should().BeTheSameInstanceAs(Z80Model.OpcodeRead);
        Z80Model.Invoking(m => m.GetSequence("nope")).Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void GetSequenceGroup()
    {
        Z80Model.GetSequenceGroup("interrupt_mode").Should().NotBeNull();
        Z80Model.Invoking(m => m.GetSequenceGroup("nope")).Should().Throw<InvalidOperationException>();
    }
}