namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Lexer token for a close bracket, ')'.
/// </summary>
internal sealed record CloseBracket : Token
{
    internal CloseBracket(int index)
        : base(index, 1)
    {
    }

    public override string ToString() => ")";
}