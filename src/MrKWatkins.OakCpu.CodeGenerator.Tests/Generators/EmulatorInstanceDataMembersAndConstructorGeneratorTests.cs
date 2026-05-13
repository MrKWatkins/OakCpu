using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorInstanceDataMembersAndConstructorGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorInstanceDataMembersAndConstructorGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput() => Verify(EmulatorInstanceDataMembersAndConstructorGenerator.Instance.Generate(Z80GeneratorContext));
}