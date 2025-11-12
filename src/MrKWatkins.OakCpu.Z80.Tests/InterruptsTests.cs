using MrKWatkins.EmulatorTestSuites.Z80;
using MrKWatkins.OakAsm.Ast;
using MrKWatkins.OakAsm.IO;
using MrKWatkins.OakAsm.Z80;
using MrKWatkins.OakCpu.Z80.Testing;
using static MrKWatkins.OakAsm.Z80.Z80Assembly;

namespace MrKWatkins.OakCpu.Z80.Tests;

// TODO: Make into a test suite.
// TODO: Review exact timings using Visual Z80 Remix, especially for instructions with overlapped reads that interrupt, e.g. the overlapped read for NOP is currently skipped.
// TODO: Interrupts after an overlapped opcode - should we run the first step of the handler? Probably. Confirm with Visual Z80 Remix.
// TODO: Test resume from HALT is in the right place.
// Some of these tests are based on https://github.com/floooh/chips-test/blob/master/tests/z80-int.c.
public sealed class InterruptsTests
{
    [Test]
    public void Mode0()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true, RegisterSP = 0x0100 };
        // ReSharper disable once RedundantArgumentDefaultValue
        z80.SetIO(new NullIO(0xFF));    // 0xFF = RST 0x38

        Load(z80,
        [
            ORG(0x0000),
            EI(),
            IM0(),
            LD(SP, 0x1122),

            Label("l0"),
            NOP(),
            NOP(),
            NOP(),
            JR("l0")
        ]);

        Load(z80,
        [
            ORG(0x0038),
            LD(A, 0x33),
            RETI()
        ]);

        // EI + IM 0 + LD SP, 0x1122.
        z80.Step(21);
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();
        z80.IM.Should().Equal(0);
        z80.RegisterSP.Should().Equal(0x1122);

        // NOP
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);

        // Trigger an interrupt.
        z80.Interrupt = true;

        // Refresh cycle in NOP.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x05);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0007);

        // Interrupt handling starts here.
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
        StepAndAssertEvent(z80, CycleType.IORead);
        StepAndAssertEvent(z80, CycleType.None);

        // Refresh cycle in interrupt handler.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x06);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Start.
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCH onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCL onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x1120);

        // Jumped to address 0x0038.
        z80.RegisterPC.Should().Equal(0x0038);

        // LD A, 0x33 - Read opcode.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // LD A, 0x33 - Read 0x33.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // LD A, 0x33 - update A. RETI - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.RegisterA.Should().Equal(0x33);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Start reading RETI second byte.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop first byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop second byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x1122);
        z80.RegisterPC.Should().Equal(0x0007);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
    }

    [Test]
    public void Mode1()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true, RegisterSP = 0x0100 };

        Load(z80,
        [
            ORG(0x0000),
            EI(),
            IM1(),

            Label("loop"),
            NOP(),
            NOP(),
            NOP(),
            JR("loop")
        ]);

        Load(z80,
        [
            ORG(0x0038),
            LD(A, 0x33),
            RETI()
        ]);

        // EI + IM 1.
        z80.Step(11);

        // IM1 - Update interrupt properties. NOP - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();
        z80.IM.Should().Equal(1);

        // NOP
        StepAndAssertEvent(z80, CycleType.None);

        // Trigger an interrupt.
        z80.Interrupt = true;

        // Refresh cycle in NOP.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x04);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0004);

        // Interrupt handling starts here.
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
        StepAndAssertEvent(z80, CycleType.IORead);
        StepAndAssertEvent(z80, CycleType.None);

        // Refresh cycle in interrupt handler.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x05);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Start.
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCH onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCL onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x00FE);

        // Jumped to address 0x0038.
        z80.RegisterPC.Should().Equal(0x0038);

        // LD A, 0x33 - Read opcode.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // LD A, 0x33 - Read 0x33.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // LD A, 0x33 - update A. RETI - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.RegisterA.Should().Equal(0x33);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Start reading RETI second byte.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop first byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop second byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x0100);
        z80.RegisterPC.Should().Equal(0x0004);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
    }

    [Test]
    public void Mode2()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true, RegisterSP = 0x0100 };
        z80.SetIO(new NullIO(0xE0));

        Load(z80,
        [
            ORG(0x0000),
            EI(),
            IM2(),
            LD(A, 0x01),
            LD(I, A),

            Label("loop"),
            NOP(),
            NOP(),
            NOP(),
            JR("loop"),

            NOP(),
            LD(A, 0x33),
            RETI()
        ]);

        z80.WriteWordToMemory(0x01E0, 0x000D);

        // EI + IM 2 + LD A, 0x01 + LD I, A.
        z80.Step(27);
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();
        z80.IM.Should().Equal(2);
        z80.RegisterI.Should().Equal(0x01);
        z80.RegisterPC.Should().Equal(0x0007);

        // NOP - Read opcode.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);

        // Trigger an interrupt.
        z80.Interrupt = true;

        // Refresh cycle in NOP.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x07);
        StepAndAssertEvent(z80, CycleType.None);

        // Interrupt handling starts here.
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
        StepAndAssertEvent(z80, CycleType.IORead);
        StepAndAssertEvent(z80, CycleType.None);

        // Refresh cycle in interrupt handler.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x08);
        StepAndAssertEvent(z80, CycleType.None);

        // Extra cycle.
        StepAndAssertEvent(z80, CycleType.None);

        // Push PCH onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Push PCL onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);

        // WZ should be set to the jump address. (0x01E0)
        z80.RegisterWZ.Should().Equal(0x01E0);

        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x00FE);

        // Read PCL.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Read PCH.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Jumped to address 0x000D.
        z80.RegisterPC.Should().Equal(0x000D);
        z80.RegisterWZ.Should().Equal(0x000D);

        // LD A, 0x33 - Read opcode.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // LD A, 0x33 - Read 0x33.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // LD A, 0x33 - update A. RETI - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.RegisterA.Should().Equal(0x33);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Start reading RETI second byte.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop first byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop second byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x0100);
        z80.RegisterPC.Should().Equal(0x0008);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
    }

    [Test]
    public void InterruptsDoNotTriggerIfDisabled()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true, RegisterSP = 0x0100 };

        Load(z80,
        [
            ORG(0x0000),
            DI(),
            NOP(),
            NOP()
        ]);

        // DI.
        z80.Step(3);
        z80.RegisterPC.Should().Equal(0x0001);

        // DI - Reset flags. NOP - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();

        // NOP.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0002);

        // Trigger an interrupt.
        z80.Interrupt = true;

        // NOP.
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // No interrupt, second NOP.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0003);

        // Interrupt should've been marked as handled.
        z80.Interrupt.Should().BeFalse();
    }

    [Test]
    public void InterruptsDoNotTriggerDuringEI()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true, RegisterSP = 0x0100 };

        Load(z80,
        [
            ORG(0x0000),
            EI(),
            IM1(),
            EI(),
            EI(),
            NOP()
        ]);

        // EI + IM 1.
        z80.Step(11);

        // IM1 - Update interrupt properties. EI - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.IFF1.Should().BeTrue();
        z80.IFF2.Should().BeTrue();
        z80.IM.Should().Equal(1);

        // EI
        StepAndAssertEvent(z80, CycleType.None);

        // Trigger an interrupt.
        z80.Interrupt = true;

        // Refresh cycle in EI.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x04);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0004);

        // No interrupt handling. Second EI.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0005);

        // No interrupt handling. Third EI.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0006);

        // Interrupt handling starts here.
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
        StepAndAssertEvent(z80, CycleType.IORead);
        StepAndAssertEvent(z80, CycleType.None);

        // Refresh cycle in interrupt handler.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x07);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Start.
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCH onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCL onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterSP.Should().Equal(0x00FE);

        // Jumped to address 0x0038.
        z80.RegisterPC.Should().Equal(0x0038);
    }

    // PC moves to the next instruction after a HALT. The next opcode is then repeatedly read, but PC is not advanced and the instruction is not executed
    // until the CPU is no longer halted. See http://www.primrosebank.net/computers/z80/z80_special_reset.htm for details.
    [Test]
    public void HALTStaysOnTheNextOpcode()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true };

        Load(z80,
        [
            ORG(0x0000),
            HALT(),
            RST20()
        ]);

        // HALT.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0001);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x01);
        StepAndAssertEvent(z80, CycleType.None);

        // HALT - set Halted. RST 0x20 - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.Halted.Should().BeTrue();

        // RST 0x20 read. PC should not advance.
        z80.RegisterPC.Should().Equal(0x0001);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x02);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x20 read again.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.RegisterPC.Should().Equal(0x0001);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x03);
        StepAndAssertEvent(z80, CycleType.None);

        // ...And so on.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        z80.RegisterPC.Should().Equal(0x0001);
    }

    [Test]
    public void InterruptsResetsHalted()
    {
        var z80 = new Z80EmulatorTestHarness { RecordCycles = true, RegisterSP = 0x0100 };

        Load(z80,
        [
            ORG(0x0000),
            EI(),
            IM1(),
            HALT(),
            RST20()
        ]);

        Load(z80,
        [
            ORG(0x0038),
            RETI()
        ]);

        // EI + IM 1 + HALT
        z80.Step(14);
        z80.Step();
        z80.RegisterPC.Should().Equal(0x0004);
        z80.RegisterR.Should().Equal(0x04);

        // HALT - set Halted. RST 0x20 - Overlapped halted cycle.
        StepAndAssertEvent(z80, CycleType.MemoryRead);

        // RST 0x20 read. PC should not advance.
        z80.RegisterPC.Should().Equal(0x0004);
        z80.Halted.Should().BeTrue();

        StepAndAssertEvent(z80, CycleType.None);

        // Trigger an interrupt.
        z80.Interrupt = true;

        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x05);
        StepAndAssertEvent(z80, CycleType.None);

        // Interrupt handling starts here.
        StepAndAssertEvent(z80, CycleType.None);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();
        StepAndAssertEvent(z80, CycleType.IORead);
        StepAndAssertEvent(z80, CycleType.None);

        // Refresh cycle in interrupt handler.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterR.Should().Equal(0x06);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Start.
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCH onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RST 0x38 - Push PCL onto the stack.
        StepAndAssertEvent(z80, CycleType.MemoryWrite);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Jumped to address 0x0038.
        z80.RegisterPC.Should().Equal(0x0038);


        // RETI - Read opcode.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // Start reading RETI second byte.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop first byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);

        // RETI - Pop second byte off stack.
        StepAndAssertEvent(z80, CycleType.MemoryRead);
        StepAndAssertEvent(z80, CycleType.None);
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0004);
        z80.IFF1.Should().BeFalse();
        z80.IFF2.Should().BeFalse();

        // RST 0x20 - Overlapped opcode read.
        StepAndAssertEvent(z80, CycleType.MemoryRead);

        // PC should be advancing again.
        StepAndAssertEvent(z80, CycleType.None);
        z80.RegisterPC.Should().Equal(0x0005);
    }

    private static void StepAndAssertEvent(Z80EmulatorTestHarness z80, CycleType type)
    {
        z80.Step();
        AssertEvent(z80, type);
    }

    private static void AssertEvent(Z80EmulatorTestHarness z80, CycleType type) => z80.Cycles.Last().Type.Should().Equal(type);

    private static void Load(Z80EmulatorTestHarness z80, [InstantHandle] IEnumerable<OakAsmNode> code)
    {
        var assembled = Z80Assembler.Instance.Assemble(new Source(code));
        var output = new Output(assembled);
        foreach (var region in output.Regions)
        {
            z80.CopyToMemory(region.Location.Address, region);
        }
    }
}