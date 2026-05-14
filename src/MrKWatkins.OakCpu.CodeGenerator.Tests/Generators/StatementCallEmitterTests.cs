using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StatementCallEmitterTests : TestFixture
{
    [Test]
    public void GenerateCall_Request_ReturnsActionRequired()
    {
        var context = new StatementGeneratorContext(CreateZ80FileGeneratorContext(), null);
        var call = ParseCallStatement("request(action.memory_read);");

        var result = StatementCallEmitter.GenerateCall(context, call).Select(statement => statement.ToNormalizedString()).ToArray();

        result.Should().SequenceEqual("return ActionRequired.MemoryRead;");
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