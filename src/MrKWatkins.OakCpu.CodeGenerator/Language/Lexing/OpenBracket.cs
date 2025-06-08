namespace MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

/// <summary>
/// Lexer token for an open bracket, '('.
/// </summary>
internal sealed record OpenBracket : Token
{
    internal OpenBracket(int index)
        : base(index, 1)
    {
    }

    public override string ToString() => "(";
}