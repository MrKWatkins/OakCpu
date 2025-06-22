namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

public readonly record struct IOEvent(ushort Port, byte Value)
{
    public override string ToString() => $"0x{Port:X4}: 0x{Value:X2}";
}