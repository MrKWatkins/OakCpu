using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStepsGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorStepsGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_DoesNotCreateNormalHandlerForOverlapOnlyPrefixedNop()
    {
        var prefixedNop = Z80GeneratorContext.Instructions.Single(i => i is { Prefix: 0xDD, Opcode: 0x00 }).Steps.Single();

        var result = EmulatorStepsGenerator.Instance.Generate(Z80GeneratorContext);

        prefixedNop.ExecutesAsOverlapOnly.Should().BeTrue();
        result.Contains($"private static void Step{prefixedNop.MethodIndex}", StringComparison.Ordinal).Should().BeFalse();
    }

    [Test]
    public void Generate_UsesBitExtractionForCarryFlag()
    {
        var result = EmulatorStepsGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("flags |= ((result) >> 8) & 1; // Set C if (result & 0x0100) == 0x0100 is true.", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("flags |= Unsafe.BitCast<bool, byte>((result & 0x0100) == 0x0100);", StringComparison.Ordinal).Should().BeFalse();
    }
}