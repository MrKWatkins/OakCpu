using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStepsInitializationGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorStepsInitializationGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_EmitsSequenceStartConstants()
    {
        var result = EmulatorStepsInitializationGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("private const ushort OpcodeReadStep0 = 0;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private const ushort HaltedStep0 = 8;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private const ushort IM0Step0 = 12;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private const ushort IM1Step0 = 17;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private const ushort IM2Step0 = 30;", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void Generate_UsesDefaultHandlerForOverlapOnlyPrefixedNop()
    {
        var prefixedNop = Z80GeneratorContext.Instructions.Single(i => i is { Prefix: 0xDD, Opcode: 0x00 }).Steps.Single();
        var prefixedNopLayout = Z80GeneratorContext.GetStepLayout(prefixedNop);
        var overlapMethodIndex = Z80GeneratorContext.GetOverlapMethodIndex(prefixedNop);

        var result = EmulatorStepsInitializationGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains($"new(default, 0, ActionRequired.None, &Overlap{overlapMethodIndex})", StringComparison.Ordinal).Should().BeTrue();
        result.Contains($"new(&Step{prefixedNopLayout.MethodIndex}, 0, ActionRequired.None, &Overlap{overlapMethodIndex})", StringComparison.Ordinal).Should().BeFalse();
    }
}