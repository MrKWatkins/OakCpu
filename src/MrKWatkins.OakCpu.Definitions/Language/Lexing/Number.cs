using System.Globalization;

namespace MrKWatkins.OakCpu.Definitions.Language.Lexing;

/// <summary>
/// Lexer token representing a number.
/// </summary>
internal sealed record Number : Token
{
    internal Number(int startIndex, int length, int value)
        : base(startIndex, length)
    {
        Value = value;
    }

    internal int Value { get; }

    public override string ToString() => Value.ToString(NumberFormatInfo.InvariantInfo);
}