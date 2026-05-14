using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StatementTransitionEmitterTests : TestFixture
{
    [Test]
    public void GenerateMoveToSequence_InNormalMode_SetsCurrentStep()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("move_to_sequence(sequence.opcode_read);");

        var result = StatementTransitionEmitter.GenerateMoveToSequence(context, call).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("emulator.currentStep = 0;");
    }

    [Test]
    public void GenerateMoveToSequence_InInstructionStepMode_SetsNextInstruction()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null).WithInstructionStepMode("nextInstruction", null, 4);
        var call = ParseCall("move_to_sequence(sequence.opcode_read);");

        var result = StatementTransitionEmitter.GenerateMoveToSequence(context, call).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("nextInstruction = 0;");
    }

    [Test]
    public void GenerateMoveToSequence_InInstructionEmulatorMode_SetsNextSequence()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null).WithInstructionEmulatorMode();
        var call = ParseCall("move_to_sequence(sequence.opcode_read);");

        var result = StatementTransitionEmitter.GenerateMoveToSequence(context, call).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("emulator.nextSequenceStep = 0;");
    }

    [Test]
    public void GenerateMoveToSequence_ThrowsForWrongArgumentCount()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("move_to_sequence();");

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementTransitionEmitter.GenerateMoveToSequence(context, call).ToArray());

        exception!.Message.Should().Equal("Calls to move_to_sequence must have exactly one argument.");
    }

    [Test]
    public void GenerateMoveToSequence_ThrowsForWrongArgumentType()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("move_to_sequence(1);");

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementTransitionEmitter.GenerateMoveToSequence(context, call).ToArray());

        exception!.Message.Should().Equal("Calls to move_to_sequence must use a sequence.<name> argument.");
    }

    [Test]
    public void GenerateMoveToInterruptMode_ThrowsForWrongArgumentCount()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("move_to_interrupt_mode();");

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementTransitionEmitter.GenerateMoveToInterruptMode(context, call).ToArray());

        exception!.Message.Should().Contain("must have exactly one argument");
    }

    [Test]
    public void GenerateMoveToSequenceGroup_ThrowsForWrongArgumentCount()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("move_to_sequence_group(sequence_group.interrupt_mode);");

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementTransitionEmitter.GenerateMoveToSequenceGroup(context, call).ToArray());

        exception!.Message.Should().Equal("Calls to move_to_sequence_group must have exactly two arguments.");
    }

    [Test]
    public void GenerateMoveToSequenceGroup_ThrowsForWrongArgumentType()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("move_to_sequence_group(1, 0);");

        var exception = Assert.Throws<InvalidOperationException>(() => _ = StatementTransitionEmitter.GenerateMoveToSequenceGroup(context, call).ToArray());

        exception!.Message.Should().Equal("Calls to move_to_sequence_group must use a sequence_group.<name> argument.");
    }

    [Test]
    public void GenerateSetOpcodeStepTable_UsesNoPrefixByDefault()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("set_opcode_step_table();");

        var result = StatementTransitionEmitter.GenerateSetOpcodeStepTable(context, call).Single().ToNormalizedString();

        result.Should().Contain("opcodeStepTable");
        result.Should().Contain(Z80GeneratorContext.Configuration.OpcodeStepTables.NoPrefix.FieldName);
    }

    [Test]
    public void GenerateSetOpcodeStepTable_UsesNumericPrefix()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("set_opcode_step_table(0xCB);");

        var result = StatementTransitionEmitter.GenerateSetOpcodeStepTable(context, call).Single().ToNormalizedString();

        result.Should().Contain("opcodeStepTable");
        result.Should().Contain(Z80GeneratorContext.Configuration.OpcodeStepTables.GetForPrefix(0xCB).FieldName);
    }

    [Test]
    public void GenerateSetOpcodeStepTable_UsesOpcodeStepTableAccess()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = new Call(PreDefinedFunction.SetOpcodeStepTable, [new OpcodeStepTableAccess(Z80GeneratorContext.Configuration.OpcodeStepTables.NoPrefix)]);

        var result = StatementTransitionEmitter.GenerateSetOpcodeStepTable(context, call).Single().ToNormalizedString();

        result.Should().Contain(Z80GeneratorContext.Configuration.OpcodeStepTables.NoPrefix.FieldName);
    }

    [Test]
    public void GenerateSetOpcodeStepTable_ThrowsForUnsupportedArgument()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCall("set_opcode_step_table(sequence.opcode_read);");

        var exception = Assert.Throws<NotSupportedException>(() => _ = StatementTransitionEmitter.GenerateSetOpcodeStepTable(context, call).ToArray());

        exception!.Message.Should().Contain("is not supported for set_opcode_step_table");
    }

    [Test]
    public void GenerateExecuteSequenceOnStart()
    {
        var result = StatementTransitionEmitter.GenerateExecuteSequenceOnStart(Z80GeneratorContext, Z80GeneratorContext.OpcodeRead, "Dispatch opcode").Single().ToNormalizedString();

        result.Should().Contain("emulator");
        result.Should().Contain("actionRequired");
    }

    [Test]
    public void GenerateExecuteOverlap()
    {
        StatementTransitionEmitter.GenerateExecuteOverlap().ToNormalizedString().Should().Equal("emulator.ExecuteOverlap();");
    }

    [Test]
    public void CreateSetStep_ForStep()
    {
        StatementTransitionEmitter.CreateSetStep(Z80GeneratorContext, Z80GeneratorContext.OpcodeRead.FirstStep).ToNormalizedString().Should().Equal("emulator.currentStep = 0;");
    }

    [Pure]
    private static Call ParseCall(string statement)
    {
        var context = new ParserContext(Z80GeneratorContext.Configuration);
        return ((CallStatement)Parser.ParseStatements(context, statement).Single()).Call;
    }
}