namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Lexer token for a semi-colon, ';'.
/// </summary>
internal sealed record SemiColon : Token
{
    internal SemiColon(int index)
        : base(index, 1)
    {
    }

    public override string ToString() => ";";
}