using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionEmulatorTablesGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InstructionEmulatorTablesGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_IncludesSequenceGroupStepTableField()
    {
        var sequenceGroup = Z80GeneratorContext.SequenceGroups.Values.OrderBy(group => group.Name).First();
        var fieldName = $"{sequenceGroup.Name.ToUpperCamelCaseFromSnakeCase()}StepTable";

        var result = InstructionEmulatorTablesGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains($"private static readonly ushort[] {fieldName};", StringComparison.Ordinal).Should().BeTrue();
        result.Contains($"{fieldName} = [", StringComparison.Ordinal).Should().BeTrue();
    }
}