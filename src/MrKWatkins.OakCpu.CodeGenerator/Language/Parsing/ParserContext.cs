using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

public sealed class ParserContext(Configuration configuration)
{
    public Configuration Configuration { get; } = configuration;

    public IReadOnlyCollection<string> Arguments { get; private set; } = [];

    public IReadOnlyList<Statement> OnInstructionComplete { get; private set; } = [];

    public Dictionary<string, TemporaryVariable> TemporaryVariables { get; private set; } = new();

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) =>
        new(Configuration)
        {
            Arguments = arguments,
            OnInstructionComplete = OnInstructionComplete,
            TemporaryVariables = TemporaryVariables
        };

    [Pure]
    public ParserContext WithOnInstructionComplete(IReadOnlyList<Statement> onInstructionComplete) =>
        new(Configuration)
        {
            Arguments = Arguments,
            OnInstructionComplete = onInstructionComplete,
            TemporaryVariables = TemporaryVariables
        };

    [Pure]
    public ParserContext WithChildVariableScope() =>
        new(Configuration)
        {
            Arguments = Arguments,
            OnInstructionComplete = OnInstructionComplete,
            TemporaryVariables = new Dictionary<string, TemporaryVariable>(TemporaryVariables)
        };
}