namespace MrKWatkins.OakCpu.Definitions.Tests;

public sealed class CpuModelTests : TestFixture
{
    [Test]
    public void Create()
    {
        var model = CpuModel.Create(Z80Yaml);

        model.Cpu.Name.Should().Equal("Z80");
        model.OpcodeRead.Should().HaveCount(4);
        model.Instructions.Should().NotBeEmpty();
        model.PrefixJumps.Keys.Should().SequenceEqual([(byte)0xCB, (byte)0xDD, (byte)0xED, (byte)0xFD]);
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