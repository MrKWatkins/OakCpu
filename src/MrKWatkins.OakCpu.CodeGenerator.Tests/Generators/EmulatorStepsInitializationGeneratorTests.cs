using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStepsInitializationGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorStepsInitializationGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_UsesDefaultHandlerForOverlapOnlyPrefixedNop()
    {
        var prefixedNop = Z80GeneratorContext.Instructions.Single(i => i is { Prefix: 0xDD, Opcode: 0x00 }).Steps.Single();
        var overlapMethodIndex = Z80GeneratorContext.GetOverlapMethodIndex(prefixedNop);

        var result = EmulatorStepsInitializationGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains($"new(default, 0, ActionRequired.None, &Overlap{overlapMethodIndex})", StringComparison.Ordinal).Should().BeTrue();
        result.Contains($"new(&Step{prefixedNop.MethodIndex}, 0, ActionRequired.None, &Overlap{overlapMethodIndex})", StringComparison.Ordinal).Should().BeFalse();
    }
}