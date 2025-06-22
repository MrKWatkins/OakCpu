namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction;

// TODO: Make general version for all tests. Maybe make more granular, e.g. to register/flag level.
[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Assertions
{
    None = 0,

    // Registers.
    // TODO: Separate A and F so we can not check A if flags checking is set.
    AF = 1 << 0,
    BC = 1 << 1,
    DE = 1 << 2,
    HL = 1 << 3,
    IX = 1 << 4,
    IY = 1 << 5,
    PC = 1 << 6,
    SP = 1 << 7,
    WZ = 1 << 8,
    I = 1 << 9,
    R = 1 << 10,
    Q = 1 << 11,
    ShadowAF = 1 << 12,
    ShadowBC = 1 << 13,
    ShadowDE = 1 << 14,
    ShadowHL = 1 << 15,
    RegistersExceptWZAndQ = AF | BC | DE | HL | IX | IY | PC | SP | I | R | ShadowAF | ShadowBC | ShadowDE | ShadowHL,
    RegistersExceptQ = RegistersExceptWZAndQ | WZ,
    Registers = RegistersExceptQ | Q,

    // Flags.
    C = 1 << 16,
    N = 1 << 17,
    PV = 1 << 18,
    X = 1 << 19,
    H = 1 << 20,
    Y = 1 << 21,
    Z = 1 << 22,
    S = 1 << 23,
    DocumentedFlags = C | N | PV | H | Z | S,
    Flags = DocumentedFlags | X | Y,

    // Interrupts.
    IFF1 = 1 << 24,
    IFF2 = 1 << 25,
    IM = 1 << 26,
    Halted = 1 << 27,
    Interrupts = IFF1 | IFF2 | IM | Halted,

    // IO.
    IOReads = 1 << 28,
    IOWrites = 1 << 29,
    IO = IOReads | IOWrites,

    Memory = 1 << 30,

    Cycles = 1 << 31,

    AllExceptCycles = Registers | Flags | IO | Interrupts | Memory,
    All = AllExceptCycles | Cycles
}