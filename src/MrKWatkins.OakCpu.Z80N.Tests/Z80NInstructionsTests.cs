using MrKWatkins.EmulatorTestSuites.Z80.Instruction.SingleStep;

namespace MrKWatkins.OakCpu.Z80N.Tests;

public sealed class Z80NInstructionsTests
{
    [TestCase("eda4", true)]        // Fuse style.
    [TestCase("ED A4", true)]       // Single step style.
    [TestCase("eda4_1", true)]      // Fuse style with more than one case for the opcode.
    [TestCase("ed23", true)]
    [TestCase("edbc", true)]
    [TestCase("eda0", false)]       // LDI, which the Z80N leaves alone.
    [TestCase("ED A0", false)]
    [TestCase("edb0", false)]       // LDIR.
    [TestCase("a4", false)]         // AND H; only the ED prefixed opcodes are the Z80N's.
    [TestCase("00", false)]
    public void IsZ80NInstruction(string testCaseId, bool expected) => Z80NInstructions.IsZ80NInstruction(testCaseId).Should().Equal(expected);

    /// <summary>
    /// Nothing is actually excluded at the moment; the Z80 instruction test suites only cover documented opcodes, so none of them
    /// test the ED prefixed opcodes the Z80N uses. The filter is there so that adding cases for them upstream turns into these
    /// tests quietly skipping them rather than the Z80N failing suites that are not about it.
    /// </summary>
    [Test]
    public void ExcludingZ80NInstructions()
    {
        var testCases = SingleStepTestSuite.Instance.GetTestCases().ToList();
        var remaining = testCases.ExcludingZ80NInstructions().ToList();

        remaining.Where(testCase => Z80NInstructions.IsZ80NInstruction(testCase.Id)).Should().BeEmpty();
        remaining.Count.Should().Equal(testCases.Count);
    }
}