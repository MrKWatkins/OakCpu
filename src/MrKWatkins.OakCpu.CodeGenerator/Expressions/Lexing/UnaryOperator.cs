namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Lexer token representing an operator, '~'.
/// </summary>
internal sealed record UnaryOperator : Token
{
    internal static readonly HashSet<char> Operators = new("~");

    internal UnaryOperator(int index, char symbol)
        : base(index, 1)
    {
        Symbol = symbol;
    }

    internal char Symbol { get; }

    public override string ToString() => new(Symbol, 1);
}