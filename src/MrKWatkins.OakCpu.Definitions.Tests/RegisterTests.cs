using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.Definitions.Tests;

public sealed class RegisterTests
{
    [Test]
    public void Create_Undocumented()
    {
        var yaml = """
                   - name: IX
                     type: u16
                     documentation: IX.
                     high:
                       name: IXH
                       type: u8
                       documentation: IXH.
                       undocumented: true
                     low:
                       name: IXL
                       type: u8
                       documentation: IXL.
                       undocumented: true
                   - name: I
                     type: u8
                     documentation: I.
                   """;

        var registers = Register.Create(YamlSerializer.Deserialize<IReadOnlyList<RegisterYaml>>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance));

        registers["IX"].Undocumented.Should().BeFalse();
        registers["IXH"].Undocumented.Should().BeTrue();
        registers["IXL"].Undocumented.Should().BeTrue();
        registers["I"].Undocumented.Should().BeFalse();
    }
}