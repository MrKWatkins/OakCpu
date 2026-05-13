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
        allGenerators.Contains(EmulatorOverlapsGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(StepStructGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(RegistersClassesGenerator.Instance).Should().BeTrue();
    }

    [Test]
    public void CreateInstructionActionCallbackParameter()
    {
        var result = TypeGenerator.CreateInstructionActionCallbackParameter();

        result.ToNormalizedString().Should().Equal("Action<ActionRequired, ushort, byte> onActionRequired");
    }
}