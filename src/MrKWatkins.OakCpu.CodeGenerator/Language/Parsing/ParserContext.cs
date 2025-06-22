using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

public sealed class ParserContext(Configuration configuration)
{
    public Configuration Configuration { get; } = configuration;

    public IReadOnlyCollection<string> Arguments { get; private set; } = [];

    public IReadOnlyList<Statement> OnInstructionComplete { get; private set; } = [];

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) =>
        new(Configuration)
        {
            Arguments = arguments,
            OnInstructionComplete = OnInstructionComplete
        };

    [Pure]
    public ParserContext WithOnInstructionComplete(IReadOnlyList<Statement> onInstructionComplete) =>
        new(Configuration)
        {
            Arguments = Arguments,
            OnInstructionComplete = onInstructionComplete
        };
}