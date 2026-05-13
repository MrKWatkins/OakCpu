using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionEmulatorStateGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InstructionEmulatorStateGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput() => Verify(InstructionEmulatorStateGenerator.Instance.Generate(Z80GeneratorContext));
}