using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

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