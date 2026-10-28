using MrKWatkins.OakCpu.Definitions.Language.Ast;
using MrKWatkins.OakCpu.Definitions.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StatementCallEmitterTests : TestFixture
{
    /// <summary>
    /// A request that is not the step's specified action, i.e. one inside an if statement, sets the action the step returns to the
    /// host rather than the step table providing it.
    /// </summary>
    [Test]
    public void GenerateCall_Request_SetsActionRequired()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCallStatement("request(action.memory_read);");

        var result = StatementCallEmitter.GenerateCall(context, call).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("actionRequired = ActionRequired.MemoryRead;");
    }

    [Test]
    public void GenerateCall_Request_CallsTheBusHandlerForTheInstructionEmulator()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null).WithInstructionEmulatorMode();
        var call = ParseCallStatement("request(action.memory_read);");

        var result = StatementCallEmitter.GenerateCall(context, call).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("handler.OnActionRequired(ActionRequired.MemoryRead, emulator.address, emulator.data);");
    }

    [Test]
    public void GenerateCall_SkipsHandleInterrupts_WhenModeSkipsIt()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null).WithoutHandleInterrupts();
        var call = ParseCallStatement("handle_interrupts();");

        var result = StatementCallEmitter.GenerateCall(context, call).ToArray();

        result.Should().BeEmpty();
    }

    [Pure]
    private static CallStatement ParseCallStatement(string statement)
    {
        var context = new ParserContext(Z80GeneratorContext.Configuration);
        return (CallStatement)Parser.ParseStatements(context, statement).Single();
    }
}