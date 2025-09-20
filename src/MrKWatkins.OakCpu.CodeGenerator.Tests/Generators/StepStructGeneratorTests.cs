using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StepStructGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => StepStructGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void GenerateOutput()
    {
        var result = StepStructGenerator.Instance.Generate(Z80GeneratorContext);

        // Validate it starts with the correct namespace
        result.StartsWith("namespace MrKWatkins.OakCpu.Z80", StringComparison.Ordinal).Should().BeTrue();

        // Validate core struct declaration with primary constructor
        result.Contains("internal unsafe readonly struct Step(", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("delegate*<Z80Emulator, ref ActionRequired, void> handler,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("ushort nextStep,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("ActionRequired actionRequired)", StringComparison.Ordinal).Should().BeTrue();

        // Validate struct fields
        result.Contains("internal readonly delegate*<Z80Emulator, ref ActionRequired, void> Handler = handler;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("internal readonly ushort NextStep = nextStep;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("internal readonly ActionRequired ActionRequired = actionRequired;", StringComparison.Ordinal).Should().BeTrue();

        // Validate proper C# structure
        result.Contains('{', StringComparison.Ordinal).Should().BeTrue();
        result.EndsWith("}", StringComparison.Ordinal).Should().BeTrue();

        // Validate reasonable length (should be around 424 characters based on earlier inspection)
        (result.Length > 400 && result.Length < 500).Should().BeTrue();
    }
}