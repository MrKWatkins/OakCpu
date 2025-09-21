using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class TypeGeneratorTests : TestFixture
{
    [Test]
    public void AllGeneratorsContainsExpectedGenerators()
    {
        var allGenerators = TypeGenerator.AllGenerators;

        allGenerators.Should().NotBeNull();
        (allGenerators.Count > 0).Should().BeTrue();
        allGenerators.Contains(ActionRequiredGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(StepStructGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(RegistersClassesGenerator.Instance).Should().BeTrue();
    }

    [Test]
    public void FileNameProperty()
    {
        ActionRequiredGenerator.Instance.FileName.Should().Equal("ActionRequired");
        StepStructGenerator.Instance.FileName.Should().Equal("StepStruct");
        RegistersClassesGenerator.Instance.FileName.Should().Equal("RegistersClasses");
    }
}