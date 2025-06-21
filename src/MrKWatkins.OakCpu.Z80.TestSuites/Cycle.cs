namespace MrKWatkins.OakCpu.Z80.TestSuites;

public readonly struct Cycle(CycleType type, ushort address, byte? data, bool isOpcodeRead = false)
{
    public CycleType Type { get; } = type;

    public ushort Address { get; } = address;

    public byte? Data { get; } = data;

    public bool IsOpcodeRead { get; } = isOpcodeRead;

    public override string ToString() => $"{Type}: 0x{Address:X4} 0x{Data:X2}";
}