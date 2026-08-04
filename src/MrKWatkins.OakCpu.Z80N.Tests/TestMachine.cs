namespace MrKWatkins.OakCpu.Z80N.Tests;

/// <summary>
/// A minimal machine for the Z80N emulator; 64K of RAM, I/O ports and Next registers that record what is written to them.
/// </summary>
internal sealed class TestMachine
{
    private readonly byte[] memory = new byte[0x10000];

    public Z80NStepEmulator Emulator { get; } = new();

    public IReadOnlyList<(ushort Port, byte Value)> IOWrites => ioWrites;

    public IReadOnlyList<(byte Register, byte Value)> NextRegisterWrites => nextRegisterWrites;

    public IReadOnlyList<ActionRequired> Actions => actions;

    public byte IOReadValue { get; set; }

    private readonly List<(ushort Port, byte Value)> ioWrites = [];
    private readonly List<(byte Register, byte Value)> nextRegisterWrites = [];
    private readonly List<ActionRequired> actions = [];

    public byte this[ushort address]
    {
        get => memory[address];
        set => memory[address] = value;
    }

    public void Load(ushort address, params byte[] bytes) => bytes.CopyTo(memory, address);

    /// <summary>
    /// Executes a single instruction, returning the number of T-states it took.
    /// </summary>
    [MustUseReturnValue]
    public int ExecuteInstruction()
    {
        var tStates = 0;
        Emulator.ExecuteInstruction(PerformAction, () => tStates++);
        return tStates;
    }

    private void PerformAction(ActionRequired action)
    {
        actions.Add(action);

        switch (action)
        {
            case ActionRequired.OpcodeRead:
            case ActionRequired.MemoryRead:
                Emulator.Data = memory[Emulator.Address];
                break;

            case ActionRequired.MemoryWrite:
                memory[Emulator.Address] = Emulator.Data;
                break;

            case ActionRequired.IORead:
                Emulator.Data = IOReadValue;
                break;

            case ActionRequired.IOWrite:
                ioWrites.Add((Emulator.Address, Emulator.Data));
                break;

            case ActionRequired.NextRegisterWrite:
                nextRegisterWrites.Add(((byte)Emulator.Address, Emulator.Data));
                break;

            case ActionRequired.None:
                break;

            default:
                throw new NotSupportedException($"The action {action} is not supported.");
        }
    }
}