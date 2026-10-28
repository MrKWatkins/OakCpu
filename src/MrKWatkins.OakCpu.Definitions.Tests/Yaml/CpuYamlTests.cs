using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.Definitions.Tests.Yaml;

public class CpuYamlTests : TestFixture
{
    [Test]
    public async Task Load()
    {
        var yaml = await LoadZ80DefinitionFileAsync("cpu.yaml");
        yaml.Cpu.Should().NotBeNull();
        yaml.OpcodeRead.Should().HaveCount(4);
    }

    [Test]
    public void Deserialize_ValidCpuWithBase()
    {
        var yaml = """
                   name: Z80N
                   base: z80
                   """;

        var cpu = YamlSerializer.Deserialize<CpuYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        cpu.Name.Should().Equal("Z80N");
        cpu.Base.Should().Equal("z80");
    }

    [Test]
    public void Deserialize_ValidCpuWithDocumentationProperties()
    {
        var yaml = """
                   name: M6502
                   display_name: 6502
                   cycles:
                     name: Cycles
                     count: actions
                     minimum: 2
                   operands:
                     - name: n
                       bytes: 1
                       documentation: An immediate byte.
                     - name: abs
                       bytes: 2
                   references:
                     - name: Manual
                       url: https://example.com/manual.pdf
                       covers: documented
                     - name: Everything
                       url: https://example.com/everything
                   temporaries:
                     result: the result
                   """;

        var cpu = YamlSerializer.Deserialize<CpuYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        cpu.DisplayName.Should().Equal("6502");
        cpu.EffectiveDisplayName.Should().Equal("6502");
        cpu.Cycles!.EffectiveName.Should().Equal("Cycles");
        cpu.Cycles.Count.Should().Equal(CycleCount.Actions);
        cpu.Cycles.EffectiveMinimum.Should().Equal(2);
        cpu.Operands.Select(operand => $"{operand.Name}={operand.EffectiveBytes}").Should().SequenceEqual("n=1", "abs=2");
        cpu.Operands[0].Documentation.Should().Equal("An immediate byte.");
        cpu.References.Select(reference => reference.Name).Should().SequenceEqual("Manual", "Everything");
        cpu.References[0].Url.Should().Equal("https://example.com/manual.pdf");
        cpu.References[0].Covers.Should().Equal(ReferenceCoverage.Documented);
        cpu.References[1].Covers.Should().Equal(ReferenceCoverage.All);
        cpu.Temporaries["result"].Should().Equal("the result");
        cpu.BaseCpu.Should().BeNull();
    }

    [Test]
    public void Deserialize_CyclesDefaults()
    {
        var yaml = """
                   name: Z80
                   cycles:
                     count: steps
                   """;

        var cpu = YamlSerializer.Deserialize<CpuYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        cpu.Cycles!.Name.Should().BeNull();
        cpu.Cycles.EffectiveName.Should().Equal("Cycles");
        cpu.Cycles.Count.Should().Equal(CycleCount.Steps);
        cpu.Cycles.Minimum.Should().BeNull();
        cpu.Cycles.EffectiveMinimum.Should().Equal(1);
    }

    [Test]
    public void Deserialize_ValidCpuWithMinimalProperties()
    {
        var yaml = """
                   name: Z80
                   """;

        var cpu = YamlSerializer.Deserialize<CpuYaml>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Verify deserialized properties
        cpu.Name.Should().Equal("Z80");
        cpu.EffectiveDisplayName.Should().Equal("Z80");
        cpu.Actions.Should().BeEmpty();
        cpu.Fields.Should().BeEmpty();
        cpu.OpcodeRead.Should().BeEmpty();
        cpu.Cycles.Should().BeNull();
        cpu.Operands.Should().BeEmpty();
        cpu.References.Should().BeEmpty();
        cpu.Temporaries.Should().BeEmpty();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(cpu, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: Z80", StringComparison.Ordinal).Should().BeTrue();
    }
}