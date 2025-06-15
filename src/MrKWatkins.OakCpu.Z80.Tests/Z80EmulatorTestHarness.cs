using FluentAssertions.Execution;
using MrKWatkins.OakCpu.Z80.TestSuites;
using MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class Z80EmulatorTestHarness : Z80TestHarness
{
    private readonly Z80Emulator emulator = new();
    private ulong noContentionUntil;

    public override ushort RegisterAF
    {
        get => emulator.Registers.AF;
        set => emulator.Registers.AF = value;
    }

    public override ushort RegisterBC
    {
        get => emulator.Registers.BC;
        set => emulator.Registers.BC = value;
    }

    public override ushort RegisterDE
    {
        get => emulator.Registers.DE;
        set => emulator.Registers.DE = value;
    }

    public override ushort RegisterHL
    {
        get => emulator.Registers.HL;
        set => emulator.Registers.HL = value;
    }

    public override ushort RegisterIX
    {
        get => emulator.Registers.IX;
        set => emulator.Registers.IX = value;
    }

    public override ushort RegisterIY
    {
        get => emulator.Registers.IY;
        set => emulator.Registers.IY = value;
    }

    public override ushort RegisterSP
    {
        get => emulator.Registers.SP;
        set => emulator.Registers.SP = value;
    }

    public override ushort RegisterPC
    {
        get => emulator.Registers.PC;
        set => emulator.Registers.PC = value;
    }

    public override ushort RegisterWZ
    {
        get => emulator.Registers.WZ;
        set => emulator.Registers.WZ = value;
    }

    public override byte RegisterI
    {
        get => emulator.Registers.I;
        set => emulator.Registers.I = value;
    }

    public override byte RegisterR
    {
        get => emulator.Registers.R;
        set => emulator.Registers.R = value;
    }

    public override ushort ShadowRegisterAF
    {
        get => emulator.Registers.Shadow.AF;
        set => emulator.Registers.Shadow.AF = value;
    }

    public override ushort ShadowRegisterBC
    {
        get => emulator.Registers.Shadow.BC;
        set => emulator.Registers.Shadow.BC = value;
    }

    public override ushort ShadowRegisterDE
    {
        get => emulator.Registers.Shadow.DE;
        set => emulator.Registers.Shadow.DE = value;
    }

    public override ushort ShadowRegisterHL
    {
        get => emulator.Registers.Shadow.HL;
        set => emulator.Registers.Shadow.HL = value;
    }

    public override bool FlagC
    {
        get => emulator.Flags.C;
        set => emulator.Flags.C = value;
    }

    public override bool FlagN
    {
        get => emulator.Flags.N;
        set => emulator.Flags.N = value;
    }

    public override bool FlagPV
    {
        get => emulator.Flags.PV;
        set => emulator.Flags.PV = value;
    }

    public override bool FlagX
    {
        get => emulator.Flags.X;
        set => emulator.Flags.X = value;
    }

    public override bool FlagH
    {
        get => emulator.Flags.H;
        set => emulator.Flags.H = value;
    }

    public override bool FlagY
    {
        get => emulator.Flags.Y;
        set => emulator.Flags.Y = value;
    }

    public override bool FlagZ
    {
        get => emulator.Flags.Z;
        set => emulator.Flags.Z = value;
    }

    public override bool FlagS
    {
        get => emulator.Flags.S;
        set => emulator.Flags.S = value;
    }

    public override bool IFF1
    {
        get => emulator.Interrupts.IFF1;
        set => emulator.Interrupts.IFF1 = value;
    }

    public override bool IFF2
    {
        get => emulator.Interrupts.IFF2;
        set => emulator.Interrupts.IFF2 = value;
    }

    public override byte IM
    {
        get => emulator.Interrupts.IM;
        set => emulator.Interrupts.IM = value;
    }

    public override bool IsHalted
    {
        get => emulator.Interrupts.IsHalted;
        set => emulator.Interrupts.IsHalted = value;
    }

    public override IDisposable CreateAssertionScope() => new AssertionScope();

    public override void AssertEqual<T>(T actual, T expected, string? message = null) => actual.Should().Be(expected, message);

    public override void AssertFail(string message) => Execute.Assertion.FailWith(message);

    public override void ExecuteStep()
    {
        var actionRequired = emulator.Step();
        PerformActionRequired(actionRequired);
        TStates++;
    }

    public override IEnumerable<TestEvent> ExecuteStepRecordingEvents()
    {
        var actionRequired = emulator.Step();
        foreach (var testEvent in PerformActionRequiredRecordingEvents(actionRequired))
        {
            yield return testEvent;
        }
        TStates++;
    }

    public override void ExecuteInstruction()
    {
        var instructionInProgress = false;
        while (true)
        {
            if (emulator.step > 1)
            {
                instructionInProgress = true;
            }
            else if (instructionInProgress)
            {
                // If we're at step 1, then we've had an overlapped read. Instruction level tests won't include this,
                // so we need to restore the PC to before the opcode read and remove any events associated with it.
                if (emulator.step == 1)
                {
                    emulator.Registers.PC--;
                    TStates--;
                }

                break;
            }

            var actionRequired = emulator.Step();
            PerformActionRequired(actionRequired);
            TStates++;
        }
    }

    // TODO: Contend events are ZX Spectrum specific, can we pull them out into the Fuse only tests? Need a none event.
    private void PerformActionRequired(ActionRequired actionRequired)
    {
        switch (actionRequired)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                emulator.Data = ReadByteFromMemory(emulator.Address);
                break;

            case ActionRequired.MemoryWrite:
                WriteByteToMemory(emulator.Address, emulator.Data);
                break;

            case ActionRequired.IoWrite:
                WriteIO(emulator.Address, emulator.Data);
                break;
        }
    }

    // TODO: Contend events are ZX Spectrum specific, can we pull them out into the Fuse only tests? Need a none event.
    private IEnumerable<TestEvent> PerformActionRequiredRecordingEvents(ActionRequired actionRequired)
    {
        switch (actionRequired)
        {
            case ActionRequired.None:
                if (TStates >= noContentionUntil)
                {
                    yield return new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data);
                    noContentionUntil++;
                }

                break;

            case ActionRequired.OpcodeRead:
                emulator.Data = ReadByteFromMemory(emulator.Address);
                yield return new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data);
                yield return new TestEvent(TestEventType.OpcodeRead, TStates, emulator.Address, emulator.Data);
                noContentionUntil += 4;

                break;

            case ActionRequired.MemoryRead:
                emulator.Data = ReadByteFromMemory(emulator.Address);
                yield return new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data);
                yield return new TestEvent(TestEventType.MemoryRead, TStates, emulator.Address, emulator.Data);
                noContentionUntil += 3;

                break;

            case ActionRequired.MemoryWrite:
                WriteByteToMemory(emulator.Address, emulator.Data);
                yield return new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data);
                yield return new TestEvent(TestEventType.MemoryWrite, TStates, emulator.Address, emulator.Data);
                noContentionUntil += 3;

                break;

            case ActionRequired.IoWrite:
                WriteIO(emulator.Address, emulator.Data);
                foreach (var ioEvent in IOContendEvents(TestEventType.IOWrite))
                {
                    yield return ioEvent;
                }
                noContentionUntil += 4;

                break;

            default:
                throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
        }
    }

    [Pure]
    private IEnumerable<TestEvent> IOContendEvents(TestEventType ioEventType)
    {
        // Based on https://sinclair.wiki.zxnet.co.uk/wiki/Contended_I/O.
        var portHighByteInRange = (emulator.Address >> 8) is >= 0x40 and <= 0x7F;
        var lowBitSet = (emulator.Address & 1) == 1;

        if (portHighByteInRange)
        {
            if (lowBitSet)
            {
                return
                [
                    new TestEvent(TestEventType.IOContend, TStates, emulator.Address, emulator.Data),
                    new TestEvent(ioEventType, TStates + 1, emulator.Address, emulator.Data),
                    new TestEvent(TestEventType.IOContend, TStates + 1, emulator.Address, emulator.Data),
                    new TestEvent(TestEventType.IOContend, TStates + 2, emulator.Address, emulator.Data),
                    new TestEvent(TestEventType.IOContend, TStates + 3, emulator.Address, emulator.Data)
                ];
            }

            return
            [
                new TestEvent(TestEventType.IOContend, TStates, emulator.Address, emulator.Data),
                new TestEvent(ioEventType, TStates + 1, emulator.Address, emulator.Data),
                new TestEvent(TestEventType.IOContend, TStates + 1, emulator.Address, emulator.Data)
            ];
        }

        if (lowBitSet)
        {
            return
            [
                new TestEvent(ioEventType, TStates + 1, emulator.Address, emulator.Data),
            ];
        }

        return
        [
            new TestEvent(ioEventType, TStates + 1, emulator.Address, emulator.Data),
            new TestEvent(TestEventType.IOContend, TStates + 1, emulator.Address, emulator.Data)
        ];
    }
}