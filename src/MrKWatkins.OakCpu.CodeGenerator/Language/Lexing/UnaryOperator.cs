using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

/// <summary>
/// Lexer token representing a unary operator.
/// </summary>
internal sealed record UnaryOperator : Token
{
    internal UnaryOperator(int index, Operator @operator)
        : base(index, @operator.Symbol.Length)
    {
        Operator = @operator;
    }

    internal Operator Operator { get; }

    public override string ToString() => Operator.Symbol;
}