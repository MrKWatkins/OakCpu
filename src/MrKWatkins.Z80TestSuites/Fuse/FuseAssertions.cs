namespace MrKWatkins.Z80TestSuites.Fuse;

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
    All = Registers | Flags | IO | Interrupts | Memory | Events
}