using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionEmulatorResetGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InstructionEmulatorResetGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput() => Verify(InstructionEmulatorResetGenerator.Instance.Generate(Z80GeneratorContext));
}