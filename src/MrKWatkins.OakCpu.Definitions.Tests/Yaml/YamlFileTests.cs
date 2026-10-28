using MrKWatkins.OakCpu.Definitions.Validation;
using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.Definitions.Tests.Yaml;

public sealed class YamlFileTests : TestFixture
{
    [TestCaseSource(nameof(YamlFileTestCases))]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void Deserialize(string path)
    {
        var bytes = File.ReadAllBytes(path);

        AssertThat.Invoking(() => _ = YamlSerializer.Deserialize<YamlFile>(bytes, YamlOptions.Instance)).Should().NotThrow();
    }

    [Pure]
    public static IEnumerable<TestCaseData> YamlFileTestCases() =>
        new DirectoryInfo(Z80DefinitionsDirectory)
            .EnumerateFiles("*.yaml")
            .Select(file => new TestCaseData(file.FullName).SetArgDisplayNames(file.Name));

    [Test]
    public void Deserialize_ValidYamlFileWithMinimalProperties()
    {
        var yaml = """
                   cpu:
                     name: TestCpu
                   interrupts:
                     handle: test_handler
                   """;

        var yamlFile = YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);

        // Verify deserialized properties
        yamlFile.Cpu.Should().NotBeNull();
        yamlFile.Cpu.Name.Should().Equal("TestCpu");
        yamlFile.Interrupts.Should().NotBeNull();
        yamlFile.Interrupts!.Handle.Should().Equal("test_handler");
        yamlFile.Registers.Should().BeEmpty();
        yamlFile.Flags.Should().BeEmpty();
        yamlFile.Instructions.Should().BeEmpty();

        // Verify round-trip serialization
        var serializedBytes = YamlSerializer.Serialize(yamlFile, YamlOptions.Instance);
        var serializedYaml = System.Text.Encoding.UTF8.GetString(serializedBytes.Span);
        serializedYaml.Contains("name: TestCpu", StringComparison.Ordinal).Should().BeTrue();
        serializedYaml.Contains("handle: test_handler", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Combine_VariantsDoNotNeedInterrupts()
    {
        var variant = DeserializeYamlFile(
            """
            cpu:
              name: TestCpuVariant
              base: TestCpu
            """);

        AssertThat.Invoking(() => _ = YamlFile.Combine(variant)).Should().NotThrow();
    }

    [Test]
    public void Combine_ThrowsIfNoInterruptsAndNoBaseCpu()
    {
        var file = DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            """);

        AssertThat.Invoking(() => _ = YamlFile.Combine(file)).Should().Throw<InvalidOperationException>().That.Should().HaveMessage("No interrupts definition found.");
    }

    [Test]
    public void Merge()
    {
        var merged = YamlFile.Merge(Base, Variant);

        merged.Cpu.Name.Should().Equal("TestCpuVariant");
        merged.Cpu.Documentation.Should().Equal("The variant.");

        // The base CPU is not itself a variant, so the merged definitions are no longer a variant of anything.
        merged.Cpu.Base.Should().BeNull();

        // Inherited from the base.
        merged.Interrupts!.Handle.Should().Equal("base_handler");
        merged.OnInstructionComplete.Should().Equal("base_on_instruction_complete");
        merged.Cpu.DefaultNextOpcode.Should().Equal(NextOpcodeMode.Overlapped);
    }

    [Test]
    public void Merge_UsesVariantDocumentationFallbackAndHooks()
    {
        var variant = DeserializeYamlFile(
            """
            cpu:
              name: TestCpuVariant
              base: TestCpu
            on_instruction_complete: variant_on_instruction_complete
            """);

        var merged = YamlFile.Merge(Base, variant);

        merged.Cpu.Documentation.Should().Equal("The base.");
        merged.OnInstructionComplete.Should().Equal("variant_on_instruction_complete");
    }

    [Test]
    public void Merge_AppendsPinsActionsAndFields()
    {
        var merged = YamlFile.Merge(Base, Variant);

        // Appending means the values generated for the base CPU's members do not change.
        merged.Cpu.Pins.Select(pin => pin.Name).Should().SequenceEqual("BASE_PIN", "VARIANT_PIN");
        merged.Cpu.Actions.Select(action => action.Name).Should().SequenceEqual("base_action", "variant_action");
        merged.Cpu.Fields.Select(field => field.Name).Should().SequenceEqual("base_field", "variant_field");
    }

    [Test]
    public void Merge_AppendsRegistersAndFlags()
    {
        var merged = YamlFile.Merge(Base, Variant);

        merged.Registers.Select(register => register.Name).Should().SequenceEqual("BR", "VR");
        merged.Flags.Select(flag => flag.Name).Should().SequenceEqual("BF", "VF");
    }

    [Test]
    public void Merge_ReplacesSequencesAndFunctionsByName()
    {
        var merged = YamlFile.Merge(Base, Variant);

        // opcode_read is inherited from the base CPU.
        merged.Sequences.Select(sequence => sequence.Name).Should().SequenceEqual("opcode_read", "kept_sequence", "replaced_sequence");
        merged.Sequences.Single(sequence => sequence.Name == "replaced_sequence").Steps.Should().SequenceEqual("VR = 1;");

        merged.Functions.Select(function => function.Name).Should().SequenceEqual("kept_function", "replaced_function");
        merged.Functions.Single(function => function.Name == "replaced_function").Expression.Should().Equal("value + 1");
    }

    [Test]
    public void Merge_OverridesIndividualOpcodes()
    {
        var merged = YamlFile.Merge(Base, Variant);

        // The base NOP keeps the opcodes the variant has not overridden.
        var nop = merged.Instructions.Single(instruction => instruction.Mnemonic == "NOP");
        nop.Opcodes.Select(opcode => opcode.Opcode).Should().SequenceEqual("0x00", "0xED 0x25");

        // The variant instructions are appended.
        merged.Instructions.Select(instruction => instruction.Mnemonic).Should().SequenceEqual("NOP", "SWAPNIB", "MIRROR", "LD BR, VR");
    }

    [Test]
    public void Merge_RemovesInstructionsWithAllOpcodesOverridden()
    {
        var merged = YamlFile.Merge(Base, Variant);

        // The variant overrides the single opcode for the base HALT, so it no longer exists.
        merged.Instructions.Select(instruction => instruction.Mnemonic).Should().NotContain("HALT");
    }

    [Test]
    public void Merge_OpcodeTablesAreDistinct()
    {
        var @base = DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
            interrupts:
              handle: base_handler
            instructions:
              - group: Base
                mnemonic: BASE
                opcode_table: DDCB
                opcodes:
                  - opcode: 0x06
            """);

        var variant = DeserializeYamlFile(
            """
            cpu:
              name: TestCpuVariant
              base: TestCpu
            instructions:
              - group: Variant
                mnemonic: VARIANT
                opcodes:
                  - opcode: 0x06
            """);

        var merged = YamlFile.Merge(@base, variant);

        // The opcodes are in different opcode tables, so the base instruction is not overridden.
        merged.Instructions.Select(instruction => instruction.Mnemonic).Should().SequenceEqual("BASE", "VARIANT");
    }

    [Test]
    public void Merge_MergedDefinitionsAreValid()
    {
        var merged = YamlFile.Merge(Base, Variant);

        AssertThat.Invoking(() => DefinitionValidation.Validate(merged)).Should().NotThrow();
    }

    private static YamlFile Base =>
        DeserializeYamlFile(
            """
            cpu:
              name: TestCpu
              documentation: The base.
              default_next_opcode: overlapped
              pins:
                - name: BASE_PIN
              actions:
                - name: base_action
                  pins: [[BASE_PIN]]
              fields:
                - name: base_field
                  type: u8
            interrupts:
              handle: base_handler
            on_instruction_complete: base_on_instruction_complete
            registers:
              - name: BR
                type: u8
            flags:
              - name: BF
                index: 0
            sequences:
              - name: opcode_read
                next_opcode: custom
                steps:
                  - Address = PC; PC = PC + 1; request(action.base_action);
              - name: kept_sequence
                steps:
                  - BR = 0;
              - name: replaced_sequence
                steps:
                  - BR = 1;
            functions:
              - name: kept_function
                type: i32_i32
                parameters: [value]
                expression: value
              - name: replaced_function
                type: i32_i32
                parameters: [value]
                expression: value
            instructions:
              - group: Base
                mnemonic: NOP
                opcodes:
                  - opcode: 0x00
                  - opcode: 0xED 0x23
                  - opcode: 0xED 0x24
                  - opcode: 0xED 0x25
                steps:
                  -
              - group: Base
                mnemonic: HALT
                opcodes:
                  - opcode: 0x76
                steps:
                  -
            """);

    private static YamlFile Variant =>
        DeserializeYamlFile(
            """
            cpu:
              name: TestCpuVariant
              documentation: The variant.
              base: TestCpu
              pins:
                - name: VARIANT_PIN
              actions:
                - name: variant_action
                  pins: [[VARIANT_PIN]]
              fields:
                - name: variant_field
                  type: u8
            registers:
              - name: VR
                type: u8
            flags:
              - name: VF
                index: 1
            sequences:
              - name: replaced_sequence
                steps:
                  - VR = 1;
            functions:
              - name: replaced_function
                type: i32_i32
                parameters: [value]
                expression: value + 1
            instructions:
              - group: Variant
                mnemonic: SWAPNIB
                opcodes:
                  - opcode: 0xED 0x23
                steps:
                  -
              - group: Variant
                mnemonic: MIRROR
                opcodes:
                  - opcode: 0xED 0x24
                steps:
                  -
              - group: Variant
                mnemonic: LD BR, VR
                opcodes:
                  - opcode: 0x76
                steps:
                  -
            """);

    [Pure]
    private static YamlFile DeserializeYamlFile(string yaml) => YamlSerializer.Deserialize<YamlFile>(System.Text.Encoding.UTF8.GetBytes(yaml), YamlOptions.Instance);
}