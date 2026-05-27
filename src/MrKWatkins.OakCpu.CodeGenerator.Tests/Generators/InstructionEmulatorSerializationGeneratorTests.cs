using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionEmulatorSerializationGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InstructionEmulatorSerializationGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_includes_span_api_and_serialized_size()
    {
        var result = InstructionEmulatorSerializationGenerator.Instance.GenerateCompilationUnit(Z80GeneratorContext).ToNormalizedString();

        result.Should().Contain("public const int SerializedSize");
        result.Should().Contain("public void Serialize(Span<byte> destination)");
        result.Should().Contain("public static Z80InstructionEmulator Deserialize(ReadOnlySpan<byte> source)");
        result.Should().Contain("public void Restore(ReadOnlySpan<byte> source)");
        result.Should().NotContain("block1");
    }
}