using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InterruptsClassGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InterruptsClassGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_InstructionHaltedSetterSchedulesHaltedSequence()
    {
        var result = InterruptsClassGenerator.Instance.GenerateFiles(Z80GeneratorContext)
            .Single(file => file.FileName == "Z80InstructionInterrupts.generated.cs")
            .Source;

        result.Contains("emulator.nextSequenceStep = value ?", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("Z80InstructionEmulator.NoNextSequenceStep", StringComparison.Ordinal).Should().BeTrue();
    }
}