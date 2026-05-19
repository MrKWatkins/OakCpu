using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

public sealed record ParserContext(Configuration Configuration)
{
    public IReadOnlyCollection<string> Arguments { get; init; } = [];

    public IReadOnlyList<Statement> OnInstructionStepsComplete { get; init; } = [];

    public Dictionary<string, TemporaryVariable> TemporaryVariables { get; init; } = new();

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) => this with { Arguments = arguments };

    [Pure]
    public ParserContext WithOnInstructionStepsComplete(IReadOnlyList<Statement> onInstructionStepsComplete) => this with { OnInstructionStepsComplete = onInstructionStepsComplete };

    [Pure]
    public ParserContext WithChildVariableScope() => this with { TemporaryVariables = new Dictionary<string, TemporaryVariable>(TemporaryVariables) };
}