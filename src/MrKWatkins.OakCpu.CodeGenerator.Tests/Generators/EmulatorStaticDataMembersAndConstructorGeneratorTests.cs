using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class EmulatorStaticDataMembersAndConstructorGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => EmulatorStaticDataMembersAndConstructorGenerator.Instance.Invoking(g => g.Generate(Z80GeneratorContext)).Should().NotThrow();
}