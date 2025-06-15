using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorInstanceDataMembersAndConstructorGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorInstanceDataMembersAndConstructorGenerator.Instance.Invoking(g => g.Generate(Z80GeneratorContext)).Should().NotThrow();
}