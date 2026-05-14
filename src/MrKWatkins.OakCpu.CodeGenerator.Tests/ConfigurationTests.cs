using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class ConfigurationTests
{
    [Test]
    public void AllDataMembers_ContainsPredefinedAndUserDefinedMembers()
    {
        var configuration = CreateConfiguration(
            [new Register("FLAGS", DataType.U8, Documentation.Empty, true, false, null, 0, true, null)],
            [CreateUserDefinedDataMember("interrupt_mode")]);

        configuration.AllDataMembers["interrupt_mode"].Name.Should().Equal("interrupt_mode");
        configuration.AllDataMembers[PreDefinedDataMember.Data.Name].Should().Equal(PreDefinedDataMember.Data);
    }

    [Test]
    public void AllFunctions_ContainsPredefinedAndUserDefinedFunctions()
    {
        var yaml = YamlSerializer.Deserialize<FunctionYaml>(
            System.Text.Encoding.UTF8.GetBytes(
                """
                name: custom
                type: u8
                expression: 1
                """),
            YamlOptions.Instance);
        var userDefinedFunctions = UserDefinedFunction.CreateDeclarations([yaml]);
        var configuration = CreateConfiguration(
            [new Register("FLAGS", DataType.U8, Documentation.Empty, true, false, null, 0, true, null)],
            [],
            userDefinedFunctions.Values.ToArray());
        UserDefinedFunction.ParseExpressions(configuration, userDefinedFunctions, [yaml]);

        configuration.AllFunctions["custom"].Should().Equal(userDefinedFunctions["custom"]);
        configuration.AllFunctions[PreDefinedFunction.Request.Name].Should().Equal(PreDefinedFunction.Request);
    }

    [Test]
    public void FlagsRegister_ThrowsWhenMissing()
    {
        var configuration = CreateConfiguration([new Register("A", DataType.U8, Documentation.Empty, false, false, null, 0, true, null)]);

        var exception = Assert.Throws<InvalidOperationException>(() => _ = configuration.FlagsRegister);

        exception!.Message.Should().Equal("No registers with flags set.");
    }

    [Test]
    public void FlagsRegister_ThrowsWhenMultiple()
    {
        var configuration = CreateConfiguration(
        [
            new Register("F1", DataType.U8, Documentation.Empty, true, false, null, 0, true, null),
            new Register("F2", DataType.U8, Documentation.Empty, true, false, null, 1, true, null)
        ]);

        var exception = Assert.Throws<InvalidOperationException>(() => _ = configuration.FlagsRegister);

        exception!.Message.Should().Equal("Multiple registers with flags set.");
    }

    [Test]
    public void ProgramCounter_ThrowsWhenMissing()
    {
        var configuration = CreateConfiguration([new Register("A", DataType.U8, Documentation.Empty, false, false, null, 0, true, null)]);

        var exception = Assert.Throws<InvalidOperationException>(() => _ = configuration.ProgramCounter);

        exception!.Message.Should().Equal("No registers with program_counter set.");
    }

    [Test]
    public void ProgramCounter_ThrowsWhenMultiple()
    {
        var configuration = CreateConfiguration(
        [
            new Register("PC1", DataType.U16, Documentation.Empty, false, true, null, 0, true, null),
            new Register("PC2", DataType.U16, Documentation.Empty, false, true, null, 2, true, null)
        ]);

        var exception = Assert.Throws<InvalidOperationException>(() => _ = configuration.ProgramCounter);

        exception!.Message.Should().Equal("Multiple registers with program_counter set.");
    }

    [Pure]
    private static Configuration CreateConfiguration(
        IReadOnlyList<Register> registers,
        IReadOnlyList<UserDefinedDataMember>? userDefinedDataMembers = null,
        IReadOnlyList<UserDefinedFunction>? userDefinedFunctions = null)
    {
        return new Configuration(
            new Dictionary<string, Action>
            {
                [Action.None.Name] = Action.None
            },
            registers.ToDictionary(r => r.Name),
            new Dictionary<string, Flag>(),
            new OpcodeStepTables([]),
            (userDefinedDataMembers ?? []).ToDictionary(d => d.Name),
            (userDefinedFunctions ?? []).ToDictionary(f => f.Name));
    }

    [Pure]
    private static UserDefinedDataMember CreateUserDefinedDataMember(string name) =>
        UserDefinedDataMember.Create(
        [
            YamlSerializer.Deserialize<FieldYaml>(
                System.Text.Encoding.UTF8.GetBytes(
                    $"""
                     name: {name}
                     type: u8
                     documentation: Interrupt mode.
                     getter: true
                     """),
                YamlOptions.Instance)
        ],
        Visibility.Private).Single();
}