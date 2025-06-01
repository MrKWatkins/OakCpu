namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Lexer token representing an operator, '+', '-', '&', '|', '^', '=' or '=='.
/// </summary>
internal sealed record BinaryOperator : Token
{
    internal static readonly HashSet<string> Operators = ["+", "-", "&", "|", "^", "=", "=="];

    internal BinaryOperator(int index, string symbol)
        : base(index, symbol.Length)
    {
        Symbol = symbol;
    }

    internal string Symbol { get; }

    public override string ToString() => Symbol;
}