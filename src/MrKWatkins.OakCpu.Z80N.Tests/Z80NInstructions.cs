using MrKWatkins.EmulatorTestSuites.Z80.Instruction;

namespace MrKWatkins.OakCpu.Z80N.Tests;

/// <summary>
/// The ED prefixed opcodes the Z80N uses for its own instructions. The Z80 test suites expect the Z80's behaviour for them, which
/// is to do nothing, so they are excluded when those suites are run against the Z80N; <see cref="InstructionTests" /> covers them
/// instead.
/// </summary>
/// <remarks>
/// The suites only cover documented opcodes, so as things stand none of them have a case for any of these and nothing is actually
/// excluded. The filter is a guard: if cases for them are added upstream the Z80N will skip them rather than fail suites that are
/// not about it.
/// </remarks>
internal static class Z80NInstructions
{
    private static readonly IReadOnlySet<string> Opcodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "ED23", // SWAPNIB
        "ED24", // MIRROR A
        "ED27", // TEST $n
        "ED28", // BSLA DE, B
        "ED29", // BSRA DE, B
        "ED2A", // BSRL DE, B
        "ED2B", // BSRF DE, B
        "ED2C", // BRLC DE, B
        "ED30", // MUL D, E
        "ED31", // ADD HL, A
        "ED32", // ADD DE, A
        "ED33", // ADD BC, A
        "ED34", // ADD HL, $nn
        "ED35", // ADD DE, $nn
        "ED36", // ADD BC, $nn
        "ED8A", // PUSH $nn
        "ED90", // OUTINB
        "ED91", // NEXTREG $n, $n
        "ED92", // NEXTREG $n, A
        "ED93", // PIXELDN
        "ED94", // PIXELAD
        "ED95", // SETAE
        "ED98", // JP (C)
        "EDA4", // LDIX
        "EDA5", // LDWS
        "EDAC", // LDDX
        "EDB4", // LDIRX
        "EDB7", // LDPIRX
        "EDBC"  // LDDRX
    };

    /// <summary>
    /// Excludes the test cases for the opcodes the Z80N uses for its own instructions.
    /// </summary>
    [Pure]
    public static IEnumerable<TTestCase> ExcludingZ80NInstructions<TTestCase>(this IEnumerable<TTestCase> testCases)
        where TTestCase : InstructionTestCase =>
        testCases.Where(testCase => !IsZ80NInstruction(testCase.Id));

    /// <summary>
    /// Determines whether a test case ID is for an opcode the Z80N uses for one of its own instructions. Test case IDs are the
    /// opcode bytes, with a suffix for suites that have more than one case per opcode, e.g. "eda4" or "ED A4" or "eda4_1".
    /// </summary>
    [Pure]
    public static bool IsZ80NInstruction(string testCaseId)
    {
        var opcode = testCaseId.Split('_')[0].Replace(" ", "", StringComparison.Ordinal);

        return Opcodes.Contains(opcode);
    }
}