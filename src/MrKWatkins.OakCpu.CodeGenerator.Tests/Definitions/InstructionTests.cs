using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Definitions;

public sealed class InstructionTests
{
    [Test]
    public void Create_SubstitutesOpcodePlaceholdersAcrossMnemonicStepsAndFlags()
    {
        var yaml = YamlSerializer.Deserialize<InstructionYaml>(
            Encoding.UTF8.GetBytes(
                """
                group: Test
                mnemonic: TEST R0,R1,RP0,RP1,C0,N0
                opcodes:
                  - opcode: 0x01
                steps:
                  - R0 = N0;
                flags:
                  Z: R0 == N0
                next_opcode: overlapped
                """),
            YamlOptions.Instance);
        typeof(InstructionYaml)
            .GetProperty(nameof(InstructionYaml.Opcodes))!
            .SetValue(
                yaml,
                new[]
                {
                    new OpcodeYaml
                    {
                        Opcode = "0x01",
                        R0 = "B",
                        R1 = "C",
                        RP0 = "BC",
                        RP1 = "DE",
                        C0 = "Z",
                        N0 = 18
                    }
                });
        var context = new ParserContext(CreateConfiguration());

        var instruction = Instruction.Create(context, [yaml]).Single();

        instruction.Mnemonic.Should().Equal("TEST B,C,BC,DE,Z,0x12");
        instruction.Steps.Should().HaveCount(1);
        instruction.Steps[0].Statements.Should().HaveCount(1);
        instruction.Steps[0].Statements[0].ToString().Should().Equal("B = 0x12");
        instruction.Flags["Z"].ToString().Should().Equal("B == 0x12");
    }

    [Pure]
    private static Configuration CreateConfiguration()
    {
        var flags = new Dictionary<string, Flag>
        {
            ["Z"] = new("Z", 1, Documentation.Empty, "Z", "NZ")
        };

        return new Configuration(
            new Dictionary<string, Action>
            {
                [Action.None.Name] = Action.None
            },
            new Dictionary<string, Register>
            {
                ["B"] = new("B", DataType.U8, Documentation.Empty, false, false, null, 0, true, null),
                ["C"] = new("C", DataType.U8, Documentation.Empty, false, false, null, 1, true, null),
                ["BC"] = new("BC", DataType.U16, Documentation.Empty, false, false, null, 2, true, null),
                ["DE"] = new("DE", DataType.U16, Documentation.Empty, false, false, null, 4, true, null)
            },
            flags,
            new OpcodeStepTables([]),
            new Dictionary<string, UserDefinedDataMember>(),
            new Dictionary<string, UserDefinedFunction>());
    }
}