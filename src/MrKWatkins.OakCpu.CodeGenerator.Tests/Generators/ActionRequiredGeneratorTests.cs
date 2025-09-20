using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class ActionRequiredGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => ActionRequiredGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void GenerateOutput()
    {
        var result = ActionRequiredGenerator.Instance.Generate(Z80GeneratorContext);

        // Validate it starts with the correct namespace
        result.StartsWith("namespace MrKWatkins.OakCpu.Z80", StringComparison.Ordinal).Should().BeTrue();

        // Validate core enum structure
        result.Contains("public enum ActionRequired", StringComparison.Ordinal).Should().BeTrue();

        // Validate all expected enum values are present in correct format
        result.Contains("None = 0,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("OpcodeRead = 1,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("MemoryRead = 2,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("MemoryWrite = 3,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("IoRead = 4,", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("IoWrite = 5", StringComparison.Ordinal).Should().BeTrue();

        // Validate proper C# structure
        result.Contains('{', StringComparison.Ordinal).Should().BeTrue();
        result.EndsWith("}", StringComparison.Ordinal).Should().BeTrue();

        // Validate reasonable length (should be around 220 characters based on earlier inspection)
        (result.Length > 200 && result.Length < 300).Should().BeTrue();
    }
}