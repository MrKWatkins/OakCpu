using FluentAssertions.Execution;
using MrKWatkins.Z80TestSuites;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class Z80EmulatorTestHarness : Z80TestHarness
{
    private readonly Z80Emulator emulator = new();
    private readonly byte[] memory = new byte[65536];

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

    public override bool IFF1 { get; set; }

    public override bool IFF2 { get; set; }

    public override byte IM { get; set; }

    public override bool IsHalted { get; set; }

    public override byte GetMemory(ushort address) => memory[address];

    public override void SetMemory(ushort address, byte value) => memory[address] = value;

    public override IDisposable CreateAssertionScope() => new AssertionScope();

    public override void AssertEqual<T>(T actual, T expected, string? message = null) => actual.Should().Be(expected, message);

    public override void ExecuteStep()
    {
        var actionRequired = emulator.Step();

        switch (actionRequired)
        {
            case ActionRequired.None:
                var previousEvent = Events.LastOrDefault();
                var previousEventFinished = previousEvent != null && TStates >= previousEvent.TStateAfter;
                if (previousEventFinished)
                {
                    AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                }
                break;

            case ActionRequired.OpcodeRead:
                emulator.Data = memory[emulator.Address];
                AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                AddEvent(new TestEvent(TestEventType.OpcodeRead, TStates, emulator.Address, emulator.Data));
                break;

            case ActionRequired.MemoryRead:
                emulator.Data = memory[emulator.Address];
                AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                AddEvent(new TestEvent(TestEventType.MemoryRead, TStates, emulator.Address, emulator.Data));
                break;

            case ActionRequired.MemoryWrite:
                memory[emulator.Address] = emulator.Data;
                AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                AddEvent(new TestEvent(TestEventType.MemoryWrite, TStates, emulator.Address, emulator.Data));
                break;

            default:
                throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
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
                    while (Events.Last().TState == TStates - 1)
                    {
                        RemoveLastEvent();
                    }

                    TStates--;
                }
                break;
            }

            var actionRequired = emulator.Step();

            switch (actionRequired)
            {
                case ActionRequired.None:
                    var previousEvent = Events.LastOrDefault();
                    var previousEventFinished = previousEvent != null && TStates >= previousEvent.TStateAfter;
                    if (previousEventFinished)
                    {
                        AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                    }
                    break;

                case ActionRequired.OpcodeRead:
                    emulator.Data = memory[emulator.Address];
                    AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                    AddEvent(new TestEvent(TestEventType.OpcodeRead, TStates, emulator.Address, emulator.Data));
                    break;

                case ActionRequired.MemoryRead:
                    emulator.Data = memory[emulator.Address];
                    AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                    AddEvent(new TestEvent(TestEventType.MemoryRead, TStates, emulator.Address, emulator.Data));
                    break;

                case ActionRequired.MemoryWrite:
                    memory[emulator.Address] = emulator.Data;
                    AddEvent(new TestEvent(TestEventType.MemoryContend, TStates, emulator.Address, emulator.Data));
                    AddEvent(new TestEvent(TestEventType.MemoryWrite, TStates, emulator.Address, emulator.Data));
                    break;

                default:
                    throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
            }
            TStates++;
        }
    }
}