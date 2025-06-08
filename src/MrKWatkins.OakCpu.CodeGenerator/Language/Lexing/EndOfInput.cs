namespace MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

/// <summary>
/// Lexer token representing the end of the expression.
/// </summary>
internal sealed record EndOfInput : Token
{
    internal EndOfInput(int index)
        : base(index, 0)
    {
    }

    public override string ToString() => "EOE";
}