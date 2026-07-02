using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Optimization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class MethodBodyOptimizationTests : TestFixture
{
    [Test]
    public void PerformAllOptimizations_RemovesCancellingAdjustments()
    {
        var result = Optimize(
            "emulator.address = emulator.PC;",
            "emulator.PC += 0x01;",
            "emulator.PC -= 1;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "emulator.address = emulator.PC;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotRemoveAdjustmentsToDifferentFields()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "emulator.SP -= 0x01;");

        Assert.That(result, Is.EqualTo([
            "emulator.PC += 0x01;",
            "emulator.SP -= 0x01;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotRemoveAdjustmentsWithDifferentValues()
    {
        var result = Optimize(
            "emulator.PC += 0x02;",
            "emulator.PC -= 1;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "ushort pc = (ushort)((emulator.PC) + 0x02);",
            "pc -= 1;",
            "emulator.PC = pc;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotRemoveSeparatedAdjustments()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "emulator.address = emulator.SP;",
            "emulator.PC -= 0x01;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "ushort pc = (ushort)((emulator.PC) + 0x01);",
            "emulator.address = emulator.SP;",
            "pc -= 0x01;",
            "emulator.PC = pc;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadAfterHalfWrite()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo([
            "emulator.W = emulator.data;",
            "emulator.PC = (ushort)(emulator.data << 8 | emulator.Z);"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotForwardReassignedSource()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.data = emulator.A;",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo([
            "emulator.W = emulator.data;",
            "emulator.data = emulator.A;",
            "emulator.PC = (ushort)(emulator.W << 8 | emulator.Z);"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotCastComposedReadInArithmeticContext()
    {
        var result = Optimize(
            "emulator.B -= 0x01;",
            "emulator.WZ = (ushort)(emulator.BC + 0x01);",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "emulator.B -= 0x01;",
            "emulator.WZ = (ushort)((emulator.B << 8 | emulator.C) + 0x01);",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotComposePairReadWithoutHalfWrite()
    {
        var result = Optimize(
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo([
            "emulator.PC = emulator.WZ;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotComposePairReadAfterWholePairWrite()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.WZ = 0x38;",
            "emulator.PC = emulator.WZ;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "byte w = emulator.data;",
            "ushort wz = 0x38;",
            "emulator.PC = wz;",
            "emulator.WZ = wz;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadAfterOneCall()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo([
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = (ushort)(emulator.W << 8 | emulator.Z);"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotComposePairReadAfterTwoCalls()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;");

        Assert.That(result, Is.EqualTo([
            "emulator.W = emulator.data;",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "handler.OnActionRequired(ActionRequired.MemoryWrite, emulator.address, emulator.data);",
            "emulator.PC = emulator.WZ;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadInsideIf()
    {
        var result = Optimize(
            "emulator.W = emulator.data;",
            "if ((emulator.F & 0b01000000) == 0b00000000) { emulator.PC = emulator.WZ; }");

        Assert.That(result[1], Does.Contain("emulator.PC = (ushort)(emulator.data << 8 | emulator.Z);"));
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
            "emulator.address = emulator.WZ;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "byte w = emulator.data;",
            "ushort wz = (ushort)((w << 8 | emulator.Z) + 0x01);",
            "emulator.address = wz;",
            "emulator.WZ = wz;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DefersRegisterWithTwoFullWrites()
    {
        var result = Optimize(
            "emulator.address = emulator.PC;",
            "emulator.PC += 0x01;",
            "handler.OnActionRequired(ActionRequired.MemoryRead, emulator.address, emulator.data);",
            "emulator.PC += 0x01;",
            "return emulator.CompleteInstruction(false, 6);");

        Assert.That(result, Is.EqualTo([
            "emulator.address = emulator.PC;",
            "ushort pc = (ushort)((emulator.PC) + 0x01);",
            "handler.OnActionRequired(ActionRequired.MemoryRead, emulator.address, emulator.data);",
            "pc += 0x01;",
            "emulator.PC = pc;",
            "return emulator.CompleteInstruction(false, 6);"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DeferralRewritesHalfReadsAsShifts()
    {
        var result = Optimize(
            "emulator.SP -= 0x01;",
            "emulator.data = emulator.SPH;",
            "emulator.SP -= 0x01;",
            "emulator.data = emulator.SPL;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "ushort sp = (ushort)((emulator.SP) - 0x01);",
            "emulator.data = (byte)(sp >> 8);",
            "sp -= 0x01;",
            "emulator.data = (byte)sp;",
            "emulator.SP = sp;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DefersPairWrittenViaHalfAndFullWrites()
    {
        var result = Optimize(
            "emulator.Z = emulator.data;",
            "emulator.WZ = 0x10;",
            "emulator.WZ = 0x20;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "byte z = emulator.data;",
            "ushort wz = 0x10;",
            "wz = 0x20;",
            "emulator.WZ = wz;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotDeferRegisterWithSingleWrite()
    {
        var result = Optimize(
            "emulator.HL = 0x10;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "emulator.HL = 0x10;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotDeferRegisterAccessedBeforeANonHandlerCall()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "if (HandleInterrupts(emulator)) { return 4; }",
            "emulator.PC += 0x01;",
            "return 0;");

        Assert.That(result[0], Is.EqualTo("emulator.PC += 0x01;"));
        Assert.That(result[^2], Is.EqualTo("emulator.PC += 0x01;"));
    }

    [Test]
    public void PerformAllOptimizations_DeferralFlushesBeforeReturnInsideIf()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "if ((emulator.F & 0x01) == 0x01) { return 1; }",
            "emulator.PC += 0x01;",
            "return 0;");

        Assert.That(result[0], Is.EqualTo("ushort pc = (ushort)((emulator.PC) + 0x01);"));
        Assert.That(result[1], Does.Contain("emulator.PC = pc;"));
        Assert.That(result[1], Does.Contain("return 1;"));
        Assert.That(result[^2], Is.EqualTo("emulator.PC = pc;"));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotDeferWhenLocalNameIsTaken()
    {
        var result = Optimize(
            "var pc = 0x10;",
            "emulator.PC += 0x01;",
            "emulator.PC += 0x01;",
            "return pc;");

        Assert.That(result, Is.EqualTo([
            "var pc = 0x10;",
            "emulator.PC += 0x01;",
            "emulator.PC += 0x01;",
            "return pc;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DeferralComposesWithPairReadComposition()
    {
        var result = Optimize(
            "emulator.PC += 0x01;",
            "emulator.W = emulator.data;",
            "emulator.PC = emulator.WZ;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "ushort pc = (ushort)((emulator.PC) + 0x01);",
            "emulator.W = emulator.data;",
            "pc = (ushort)(emulator.data << 8 | emulator.Z);",
            "emulator.PC = pc;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DeferralDropsDeadIntermediateHalfWrites()
    {
        // The LD (nn), A shape: the first W write only feeds the address and is overwritten before the
        // instruction ends, so no store for it ever reaches the field.
        var result = Optimize(
            "emulator.W = emulator.data;",
            "emulator.address = emulator.WZ;",
            "emulator.Z += 0x01;",
            "emulator.W = emulator.A;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "byte w = emulator.data;",
            "emulator.address = (ushort)(w << 8 | emulator.Z);",
            "byte z = (byte)(emulator.Z + 0x01);",
            "w = emulator.A;",
            "emulator.W = w;",
            "emulator.Z = z;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DoesNotDeferFlagsPairs()
    {
        // The flags pair is written by almost every instruction but essentially never read at 16 bits, so tracking
        // it would produce the same byte stores with pointless locals in between.
        var result = Optimize(
            "emulator.A = emulator.data;",
            "emulator.F = 0x10;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "emulator.A = emulator.data;",
            "emulator.F = 0x10;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_DeferralFlushesTransientPairsAsByteStores()
    {
        var result = Optimize(
            "emulator.Z = emulator.data;",
            "emulator.W = emulator.A;",
            "return 0;");

        Assert.That(result, Is.EqualTo([
            "byte z = emulator.data;",
            "byte w = emulator.A;",
            "emulator.W = w;",
            "emulator.Z = z;",
            "return 0;"
        ]));
    }

    [Test]
    public void PerformAllOptimizations_ComposesPairReadForAutoNamedHalves()
    {
        var result = Optimize(
            "emulator.PCL = emulator.data;",
            "emulator.address = emulator.PC;");

        Assert.That(result, Is.EqualTo([
            "emulator.PCL = emulator.data;",
            "emulator.address = (ushort)(emulator.PCH << 8 | emulator.data);"
        ]));
    }

    [Pure]
    private static IReadOnlyList<string> Optimize(params string[] statements)
    {
        var context = CreateZ80FileGeneratorContext();
        var parsed = statements.Select(statement => SyntaxFactory.ParseStatement(statement)).ToList();

        var optimized = MethodBodyOptimization.PerformInstructionMethodOptimizations(context, parsed);

        return optimized.Select(statement => statement.NormalizeWhitespace().ToFullString().ReplaceLineEndings(" ").Replace("  ", " ", StringComparison.Ordinal)).ToList();
    }
}