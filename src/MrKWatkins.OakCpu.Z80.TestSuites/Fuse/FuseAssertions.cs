namespace MrKWatkins.OakCpu.Z80.TestSuites.Fuse;

// TODO: Make general version for all tests. Maybe make more granular, e.g. to register/flag level.
[Flags]
public enum FuseAssertions
{
    None = 0,
    Registers = 1,
    Flags = 2,
    IO = 4,
    Interrupts = 8,
    Memory = 16,
    Events = 32,
    AllExceptEvents = Registers | Flags | IO | Interrupts | Memory,
    AllExceptRegisters = Flags | IO | Interrupts | Memory | Events,
    All = Registers | Flags | IO | Interrupts | Memory | Events
}