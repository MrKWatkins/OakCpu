using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionEmulatorInterruptsGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InstructionEmulatorInterruptsGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_UsesInterruptHandleFromYaml()
    {
        var result = InstructionEmulatorInterruptsGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("if (emulator.interrupt && emulator.iff1)", StringComparison.Ordinal).Should().BeFalse();
        result.Contains("if (emulator.interrupt & emulator.iff1)", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("emulator.interrupt = false;", StringComparison.Ordinal).Should().BeFalse();
        result.Contains("emulator.nextSequenceStep = ", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("return false;", StringComparison.Ordinal).Should().BeTrue();
    }
}