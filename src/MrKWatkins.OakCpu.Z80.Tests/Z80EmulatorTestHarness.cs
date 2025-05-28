using FluentAssertions.Execution;
using MrKWatkins.Z80TestSuites;

namespace MrKWatkins.OakCpu.Z80.Tests;

public sealed class Z80EmulatorTestHarness : Z80TestHarness
{
    private readonly Z80Emulator emulator = new();
    private readonly byte[] memory = new byte[65536];

    public override ushort AF
    {
        get => emulator.Registers.AF;
        set => emulator.Registers.AF = value;
    }

    public override ushort BC
    {
        get => emulator.Registers.BC;
        set => emulator.Registers.BC = value;
    }

    public override ushort DE
    {
        get => emulator.Registers.DE;
        set => emulator.Registers.DE = value;
    }

    public override ushort HL
    {
        get => emulator.Registers.HL;
        set => emulator.Registers.HL = value;
    }

    public override ushort IX
    {
        get => emulator.Registers.IX;
        set => emulator.Registers.IX = value;
    }

    public override ushort IY
    {
        get => emulator.Registers.IY;
        set => emulator.Registers.IY = value;
    }

    public override ushort SP
    {
        get => emulator.Registers.SP;
        set => emulator.Registers.SP = value;
    }

    public override ushort PC
    {
        get => emulator.Registers.PC;
        set => emulator.Registers.PC = value;
    }

    public override ushort WZ
    {
        get => emulator.Registers.WZ;
        set => emulator.Registers.WZ = value;
    }

    public override byte I
    {
        get => emulator.Registers.I;
        set => emulator.Registers.I = value;
    }

    public override byte R
    {
        get => emulator.Registers.R;
        set => emulator.Registers.R = value;
    }

    public override ushort ShadowAF
    {
        get => emulator.Registers.Shadow.AF;
        set => emulator.Registers.Shadow.AF = value;
    }

    public override ushort ShadowBC
    {
        get => emulator.Registers.Shadow.BC;
        set => emulator.Registers.Shadow.BC = value;
    }

    public override ushort ShadowDE
    {
        get => emulator.Registers.Shadow.DE;
        set => emulator.Registers.Shadow.DE = value;
    }

    public override ushort ShadowHL
    {
        get => emulator.Registers.Shadow.HL;
        set => emulator.Registers.Shadow.HL = value;
    }

    public override bool IFF1 { get; set; }

    public override bool IFF2 { get; set; }

    public override byte IM { get; set; }

    public override bool IsHalted { get; set; }

    public override byte GetMemory(ushort address) => memory[address];

    public override void SetMemory(ushort address, byte value) => memory[address] = value;

    public override IDisposable CreateAssertionScope() => new AssertionScope();

    public override void AssertEqual<T>(T actual, T expected, string? message = null) => actual.Should().Be(expected, message);

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
                // If we're at step 1 then we've had an overlapped read. Adjust PC down by 1 as instruction level tests won't take that into account.
                if (emulator.step == 1)
                {
                    emulator.Registers.PC--;
                }
                break;
            }

            var actionRequired = emulator.Step();
            TStates++;

            switch (actionRequired)
            {
                case ActionRequired.None:
                    break;

                case ActionRequired.MemoryRead:
                    emulator.Data = memory[emulator.Address];
                    break;

                case ActionRequired.MemoryWrite:
                    memory[emulator.Address] = emulator.Data;
                    break;

                default:
                    throw new NotSupportedException($"The {nameof(ActionRequired)} {actionRequired} is not supported.");
            }
        }
    }
}