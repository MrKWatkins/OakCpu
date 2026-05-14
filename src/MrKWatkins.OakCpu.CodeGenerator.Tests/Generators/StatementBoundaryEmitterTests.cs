namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StatementBoundaryEmitterTests : TestFixture
{
    [Test]
    public void GenerateHandled_ThrowsWhenStepPresent()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Z80GeneratorContext.Instructions.First().Steps.First());

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementBoundaryEmitter.GenerateHandled(context).ToArray());

        exception!.Message.Should().Equal("Cannot use handled() inside an instruction.");
    }

    [Test]
    public void GenerateHandled_InNormalMode_AssignsActionRequiredAndReturnsTrue()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);

        var result = StatementBoundaryEmitter.GenerateHandled(context).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("actionRequired = ActionRequired.None;", "return true;");
    }

    [Test]
    public void GenerateHandled_InInstructionEmulatorMode_ReturnsTrueOnly()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null).WithInstructionEmulatorMode();

        var result = StatementBoundaryEmitter.GenerateHandled(context).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("return true;");
    }

    [Test]
    public void GenerateBoundaryStatements_ReturnsEmpty_WhenStepDoesNotQueueOverlap()
    {
        var step = Z80GeneratorContext.OpcodeRead.FirstStep;
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), step);

        var result = StatementBoundaryEmitter.GenerateBoundaryStatements(context).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().BeEmpty();
    }

    [Test]
    public void GenerateHandleInterrupts_InInstructionCompletionMode_ReturnsDirectCall()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Z80GeneratorContext.OpcodeRead.FirstStep).WithInstructionCompletionMode("instructionUpdatesFlags");

        var result = StatementBoundaryEmitter.GenerateHandleInterrupts(context).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().HaveCount(1);
        result[0].Should().Contain("HandleInterrupts");
        result[0].Should().NotContain("return");
    }

    [Test]
    public void GenerateInstructionComplete_ThrowsInInstructionEmulatorMode()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null).WithInstructionEmulatorMode();

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementBoundaryEmitter.GenerateInstructionComplete(context).ToArray());

        exception!.Message.Should().Equal("instruction_complete is only supported when generating instruction-emulator steps.");
    }

    [Test]
    public void GenerateInstructionComplete_ReturnsCompleteInstructionCall_InInstructionStepModeForInstruction()
    {
        var step = Z80GeneratorContext.Instructions.First().Steps.First();
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), step).WithInstructionStepMode(null, null, 4);

        var result = StatementBoundaryEmitter.GenerateInstructionComplete(context).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().HaveCount(1);
        result[0].Should().Contain("emulator.CompleteInstruction");
        result[0].Should().Contain("5");
    }

    [Test]
    public void GenerateInstructionComplete_InNormalMode_EmitsInstructionCompleteStatementsAndResetsStep()
    {
        var step = Z80GeneratorContext.Instructions.First().Steps.First();
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), step);

        var result = StatementBoundaryEmitter.GenerateInstructionComplete(context).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().NotBeEmpty();
        result[^1].Should().Contain("currentStep =");
    }

    [Test]
    public void GenerateHandleInterruptsAndReturnIfHandled_ReturnsTStates_WhenInstructionStepIsNotInstruction()
    {
        var step = Z80GeneratorContext.OpcodeRead.FirstStep;
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), step).WithInstructionStepMode(null, null, 4);

        var result = StatementBoundaryEmitter.GenerateHandleInterruptsAndReturnIfHandled(context).ToNormalizedString();

        result.Should().Contain("HandleInterrupts(emulator)");
        result.Should().Contain("return 5;");
    }
}