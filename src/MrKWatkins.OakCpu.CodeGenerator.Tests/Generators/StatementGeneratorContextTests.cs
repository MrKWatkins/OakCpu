using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StatementGeneratorContextTests : TestFixture
{
    [Test]
    public void Constructor_WithoutStep()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        context.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        context.Step.Should().BeNull();
        context.ArgumentScope.Should().BeEmpty();
        context.InitializedTemporaryVariables.Should().BeEmpty();
        context.InBooleanContext.Should().BeFalse();
        context.Parent.Should().BeNull();
        context.Mode.Should().Equal(StatementGenerationMode.Normal);
        context.Mode.SequenceTransitionTarget.Should().Equal(SequenceTransitionTarget.CurrentStep);
        context.SkipHandleInterrupts.Should().BeFalse();
        context.InstructionCompletionMode.Should().BeFalse();
        context.InstructionEmulatorMode.Should().BeFalse();
        context.InstructionStepMode.Should().BeFalse();
        context.InstructionStep.Should().BeNull();
        context.InstructionUpdatesFlagsParameterName.Should().BeNull();
    }

    [Test]
    public void Constructor_WithStep()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Step);
        context.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        context.Step.Should().BeTheSameInstanceAs(Step);
        context.ArgumentScope.Should().BeEmpty();
        context.InitializedTemporaryVariables.Should().BeEmpty();
        context.InBooleanContext.Should().BeFalse();
        context.Parent.Should().BeNull();
        context.Mode.Should().Equal(StatementGenerationMode.Normal);
        context.Mode.SequenceTransitionTarget.Should().Equal(SequenceTransitionTarget.CurrentStep);
        context.SkipHandleInterrupts.Should().BeFalse();
        context.InstructionCompletionMode.Should().BeFalse();
        context.InstructionEmulatorMode.Should().BeFalse();
        context.InstructionStepMode.Should().BeFalse();
        context.InstructionStep.Should().BeNull();
        context.InstructionUpdatesFlagsParameterName.Should().BeNull();
    }

    [Test]
    public void WithArguments()
    {
        var context = CreateInstructionStepContext();

        var parameters = new[] { "x", "y" };
        var arguments = new[] { CreateExpression("5"), CreateExpression("true") };

        var newContext = context.WithArguments(parameters, arguments);
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.InitializedTemporaryVariables.Should().BeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.InBooleanContext.Should().BeFalse();
        newContext.Parent.Should().BeNull();
        newContext.Mode.Should().BeOfType<StatementGenerationMode.InstructionStepMode>();
        newContext.InstructionStep.Should().BeTheSameInstanceAs(context.InstructionStep);

        newContext.ArgumentScope.Should().HaveCount(2);
        newContext.ArgumentScope["x"].Should().BeTheSameInstanceAs(arguments[0]);
        newContext.ArgumentScope["y"].Should().BeTheSameInstanceAs(arguments[1]);
    }

    [Test]
    public void WithBooleanContext()
    {
        var context = CreateInstructionStepContext();

        var newContext = context.WithBooleanContext();
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.ArgumentScope.Should().BeTheSameInstanceAs(context.ArgumentScope);
        newContext.InitializedTemporaryVariables.Should().BeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.Parent.Should().BeNull();
        newContext.Mode.Should().BeOfType<StatementGenerationMode.InstructionStepMode>();
        newContext.InstructionStep.Should().BeTheSameInstanceAs(context.InstructionStep);

        newContext.InBooleanContext.Should().BeTrue();
    }

    [Test]
    public void WithChildVariableScope()
    {
        var context = CreateInstructionStepContext();
        context.InitializedTemporaryVariables.Add("test");

        var newContext = context.WithChildVariableScope();
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.ArgumentScope.Should().BeTheSameInstanceAs(context.ArgumentScope);
        newContext.InBooleanContext.Should().BeFalse();
        newContext.Parent.Should().BeNull();
        newContext.Mode.Should().BeOfType<StatementGenerationMode.InstructionStepMode>();
        newContext.InstructionStep.Should().BeTheSameInstanceAs(context.InstructionStep);

        newContext.InitializedTemporaryVariables.Should().NotBeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.InitializedTemporaryVariables.Should().HaveCount(1);
        newContext.InitializedTemporaryVariables.Contains("test").Should().BeTrue();
    }

    [Test]
    public void WithParentExpression()
    {
        var context = CreateInstructionStepContext();

        var parent = CreateExpression("10");

        var newContext = context.WithParentExpression(parent);
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.ArgumentScope.Should().BeTheSameInstanceAs(context.ArgumentScope);
        newContext.InitializedTemporaryVariables.Should().BeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.InBooleanContext.Should().BeFalse();
        newContext.Mode.Should().BeOfType<StatementGenerationMode.InstructionStepMode>();
        newContext.InstructionStep.Should().BeTheSameInstanceAs(context.InstructionStep);

        newContext.Parent.Should().BeTheSameInstanceAs(parent);
    }

    [Test]
    public void WithoutHandleInterrupts()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Step);

        var newContext = context.WithoutHandleInterrupts();

        newContext.Mode.Should().Equal(StatementGenerationMode.Overlap);
        newContext.Mode.SequenceTransitionTarget.Should().Equal(SequenceTransitionTarget.CurrentStep);
        newContext.SkipHandleInterrupts.Should().BeTrue();
        newContext.InstructionCompletionMode.Should().BeFalse();
        newContext.InstructionEmulatorMode.Should().BeFalse();
        newContext.InstructionStepMode.Should().BeFalse();
        newContext.InstructionStep.Should().BeNull();
        newContext.InstructionUpdatesFlagsParameterName.Should().BeNull();
    }

    [Test]
    public void WithInstructionEmulatorMode()
    {
        var context = CreateInstructionStepContext();

        var newContext = context.WithInstructionEmulatorMode();

        newContext.Mode.Should().Equal(StatementGenerationMode.InstructionEmulator);
        newContext.Mode.SequenceTransitionTarget.Should().Equal(SequenceTransitionTarget.NextSequence);
        newContext.SkipHandleInterrupts.Should().BeFalse();
        newContext.InstructionCompletionMode.Should().BeFalse();
        newContext.InstructionEmulatorMode.Should().BeTrue();
        newContext.InstructionStepMode.Should().BeFalse();
        newContext.InstructionStep.Should().BeNull();
        newContext.InstructionUpdatesFlagsParameterName.Should().BeNull();
    }

    [Test]
    public void WithInstructionStepMode()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Step);

        var newContext = context.WithInstructionStepMode("nextInstruction", Step, 7);

        newContext.Mode.Should().BeOfType<StatementGenerationMode.InstructionStepMode>();
        newContext.Mode.SequenceTransitionTarget.Should().Equal(SequenceTransitionTarget.NextInstruction);
        newContext.SkipHandleInterrupts.Should().BeFalse();
        newContext.InstructionCompletionMode.Should().BeFalse();
        newContext.InstructionEmulatorMode.Should().BeTrue();
        newContext.InstructionStepMode.Should().BeTrue();
        newContext.InstructionUpdatesFlagsParameterName.Should().BeNull();

        var instructionStep = newContext.RequiredInstructionStep;
        instructionStep.NextInstructionVariableName.Should().Equal("nextInstruction");
        instructionStep.ExitOverlapStep.Should().BeTheSameInstanceAs(Step);
        instructionStep.TStatesBeforeStep.Should().Equal(7);
    }

    [Test]
    public void WithInstructionCompletionMode()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);

        var newContext = context.WithInstructionCompletionMode("instructionUpdatesFlags");

        newContext.Mode.Should().BeOfType<StatementGenerationMode.InstructionCompletionMode>();
        newContext.Mode.SequenceTransitionTarget.Should().Equal(SequenceTransitionTarget.NextSequence);
        newContext.SkipHandleInterrupts.Should().BeFalse();
        newContext.InstructionCompletionMode.Should().BeTrue();
        newContext.InstructionEmulatorMode.Should().BeFalse();
        newContext.InstructionStepMode.Should().BeFalse();
        newContext.InstructionStep.Should().BeNull();
        newContext.InstructionUpdatesFlagsParameterName.Should().Equal("instructionUpdatesFlags");
    }

    [Test]
    public void RequiredInstructionStep_ThrowsOutsideInstructionStepMode()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Step);

        Assert.Throws<InvalidOperationException>(() => _ = context.RequiredInstructionStep);
    }

    [Pure]
    private static Expression CreateExpression(string text)
    {
        var context = new ParserContext(Z80GeneratorContext.Configuration);
        return Parser.ParseExpression(context, text);
    }

    private static StatementGeneratorContext CreateInstructionStepContext() => new StatementGeneratorContext(CreateZ80FileGeneratorContext(), Step).WithInstructionStepMode("nextInstruction", Step, 7);

    private static Step Step => Z80GeneratorContext.Instructions[0].FirstStep;
}