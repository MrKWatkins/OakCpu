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
}