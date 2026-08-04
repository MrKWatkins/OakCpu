using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class ActionRequiredGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => ActionRequiredGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public Task GenerateOutput()
    {
        var result = ActionRequiredGenerator.Instance.Generate(Z80GeneratorContext);
        return Verify(result);
    }

    /// <summary>
    /// The Z80N's action is appended to those it inherits from the Z80, so the values generated for the Z80's actions are unchanged.
    /// </summary>
    [Test]
    public Task GenerateOutput_Z80N()
    {
        var result = ActionRequiredGenerator.Instance.Generate(Z80NGeneratorContext);
        return Verify(result);
    }
}