namespace MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

/// <summary>
/// Lexer token for a comma, ','.
/// </summary>
internal sealed record Comma : Token
{
    internal Comma(int index)
        : base(index, 1)
    {
    }

    public override string ToString() => ",";
}