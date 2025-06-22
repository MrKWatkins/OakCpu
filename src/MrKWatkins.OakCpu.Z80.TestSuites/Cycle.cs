namespace MrKWatkins.OakCpu.Z80.TestSuites;

public sealed class Cycle(CycleType type, ulong index, ushort address, byte? data, bool isOpcodeRead = false)
{
    public CycleType Type { get; } = type;

    public ulong Index { get; } = index;

    public ushort Address { get; } = address;

    public byte? Data { get; } = data;

    public bool IsOpcodeRead { get; } = isOpcodeRead;

    public override string ToString() => $"{Type}: 0x{Address:X4} 0x{Data:X2}";
}