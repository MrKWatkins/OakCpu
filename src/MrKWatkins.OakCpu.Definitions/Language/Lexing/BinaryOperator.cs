using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.Definitions.Language.Lexing;

/// <summary>
/// Lexer token representing a binary operator.
/// </summary>
internal sealed record BinaryOperator : Token
{
    internal BinaryOperator(int index, Operator @operator)
        : base(index, @operator.Symbol.Length)
    {
        Operator = @operator;
    }

    internal Operator Operator { get; }

    public override string ToString() => Operator.Symbol;
}