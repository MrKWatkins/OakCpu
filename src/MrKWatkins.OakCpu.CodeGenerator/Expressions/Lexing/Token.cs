namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Base class for a lexer token.
/// </summary>
public abstract record Token
{
    private protected Token(int startIndex, int length)
    {
        StartIndex = startIndex;
        Length = length;
    }

    /// <summary>
    /// The start index for the token in the input stream.
    /// </summary>
    internal int StartIndex { get; }

    /// <summary>
    /// The length of the token.
    /// </summary>
    internal int Length { get; }
}