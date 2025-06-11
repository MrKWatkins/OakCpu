namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel;

// TODO: Make general version for all tests. Maybe make more granular, e.g. to register/flag level.
[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Assertions
{
    None = 0,

    // Registers.
    AF = 1 << 0,
    BC = 1 << 1,
    DE = 1 << 2,
    HL = 1 << 3,
    IX = 1 << 4,
    IY = 1 << 5,
    PC = 1 << 6,
    SP = 1 << 7,
    I = 1 << 8,
    R = 1 << 9,
    ShadowAF = 1 << 10,
    ShadowBC = 1 << 11,
    ShadowDE = 1 << 12,
    ShadowHL = 1 << 13,
    WZ = 1 << 14,
    RegistersExceptWZ = AF | BC | DE | HL | IX | IY | PC | SP | I | R | ShadowAF| ShadowBC | ShadowDE | ShadowHL,
    Registers = RegistersExceptWZ | WZ,

    // Flags.
    C = 1 << 15,
    N = 1 << 16,
    PV = 1 << 17,
    X = 1 << 18,
    H = 1 << 19,
    Y = 1 << 20,
    Z = 1 << 21,
    S = 1 << 22,
    DocumentedFlags = C | N | PV | H | Z | S,
    Flags = DocumentedFlags | X | Y,

    // Interrupts.
    IFF1 = 1 << 23,
    IFF2 = 1 << 24,
    IM = 1 << 25,
    Halted = 1 << 26,
    Interrupts = IFF1 | IFF2 | IM | Halted,

    // IO.
    IOReads = 1 << 27,
    IOWrites = 1 << 28,
    IO = IOReads | IOWrites,

    Memory = 1 << 29,

    Events = 1 << 30,

    AllExceptEvents = Registers | Flags | IO | Interrupts | Memory,
    All = AllExceptEvents | Events
}