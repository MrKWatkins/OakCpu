using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

public sealed record ParserContext(Configuration Configuration)
{
    public IReadOnlyCollection<string> Arguments { get; init; } = [];

    public IReadOnlyList<Statement> OnInstructionComplete { get; init; } = [];

    public Dictionary<string, TemporaryVariable> TemporaryVariables { get; init; } = new();

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) => this with { Arguments = arguments };

    [Pure]
    public ParserContext WithOnInstructionComplete(IReadOnlyList<Statement> onInstructionComplete) => this with { OnInstructionComplete = onInstructionComplete };

    [Pure]
    public ParserContext WithChildVariableScope() => this with { TemporaryVariables = new Dictionary<string, TemporaryVariable>(TemporaryVariables) };
}