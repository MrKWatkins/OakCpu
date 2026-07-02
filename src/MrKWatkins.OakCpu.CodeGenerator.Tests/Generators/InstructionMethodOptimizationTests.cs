using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods.Optimization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionMethodOptimizationTests : TestFixture
{
    [Test]
    public void PerformAllOptimizations_RemovesCancellingAdjustments()
    {
        var result = Optimize(
            "emulator.address = emulator.PC;",
            "emulator.PC += 0x01;",
            "emulator.PC -= 1;",
            "return 0;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.address = emulator.PC;",
            "return 0;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotRemoveAdjustmentsToDifferentFields()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "emulator.SP -= 0x01;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.PC += 0x01;",
            "emulator.SP -= 0x01;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotRemoveAdjustmentsWithDifferentValues()
    {
        var result = Optimize(
            "emulator.PC += 0x02;",
            "emulator.PC -= 1;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.PC += 0x02;",
            "emulator.PC -= 1;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotRemoveSeparatedAdjustments()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "emulator.address = emulator.SP;",
            "emulator.PC -= 0x01;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.PC += 0x01;",
            "emulator.address = emulator.SP;",
            "emulator.PC -= 0x01;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadAfterHalfWrite()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.W = emulator.data;",
            "emulator.PC = (ushort)(emulator.W << 8 | emulator.Z);"
        }));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotComposePairReadWithoutHalfWrite()
    {
        var result = Optimize(
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.PC = emulator.WZ;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotComposePairReadAfterWholePairWrite()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.WZ = 0x38;",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.W = emulator.data;",
            "emulator.WZ = 0x38;",
            "emulator.PC = emulator.WZ;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadAfterOneCall()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = (ushort)(emulator.W << 8 | emulator.Z);"
        }));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotComposePairReadAfterTwoCalls()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadInsideIf()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "if ((emulator.F & 0b01000000) == 0b00000000) { emulator.PC = emulator.WZ; }");

        Assert.That(result[1], Does.Contain("emulator.PC = (ushort)(emulator.W << 8 | emulator.Z);"));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotCountCallsInsideTerminatingIf()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "if ((emulator.F & 0b01000000) == 0b01000000) { return emulator.CompleteInstruction(false, 6); }",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;");

        Assert.That(result[^1], Is.EqualTo("emulator.PC = (ushort)(emulator.W << 8 | emulator.Z);"));
    }

    [Test]
    public void PerformAllOptimizations_ComposesCompoundPairAssignmentAfterHalfWrite()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.WZ += 0x01;",
            "emulator.address = emulator.WZ;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.W = emulator.data;",
            "emulator.WZ = (ushort)((emulator.W << 8 | emulator.Z) + 0x01);",
            // The compound assignment wrote the whole pair, so the following read is left alone.
            "emulator.address = emulator.WZ;"
        }));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadForAutoNamedHalves()
    {
        var result = Optimize(
            "emulator.PCL = emulator.data;",
            "emulator.address = emulator.PC;");

        Assert.That(result, Is.EqualTo(new[]
        {
            "emulator.PCL = emulator.data;",
            "emulator.address = (ushort)(emulator.PCH << 8 | emulator.PCL);"
        }));
    }

    [Pure]
    private static IReadOnlyList<string> Optimize(params string[] statements)
    {
        var context = CreateZ80FileGeneratorContext();
        var parsed = statements.Select(statement => SyntaxFactory.ParseStatement(statement)).ToList<StatementSyntax>();

        var optimized = InstructionMethodOptimization.PerformAllOptimizations(context, parsed);

        return optimized.Select(statement => statement.NormalizeWhitespace().ToFullString().ReplaceLineEndings(" ").Replace("  ", " ", StringComparison.Ordinal)).ToList();
    }
}