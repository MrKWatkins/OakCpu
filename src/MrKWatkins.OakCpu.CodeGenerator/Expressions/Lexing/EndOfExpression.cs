namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Lexer token representing the end of the expression.
/// </summary>
internal sealed record EndOfExpression : Token
{
    internal EndOfExpression(int index)
        : base(index, 0)
    {
    }

    public override string ToString() => "EOE";
}