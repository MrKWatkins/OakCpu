using MrKWatkins.OakCpu.CodeGenerator.Generators;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

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
    public void InstructionActionCallback()
    {
        var result = Parameter.Syntax.InstructionActionCallback();

        result.ToNormalizedString().Should().Equal("Action<ActionRequired, ushort, byte> onActionRequired");
    }
}