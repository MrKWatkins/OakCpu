namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Lexing;

/// <summary>
/// Lexer token representing an arbitrary identifier.
/// </summary>
internal sealed record Identifier : Token
{
    internal Identifier(int startIndex, string name)
        : base(startIndex, name.Length)
    {
        Name = name;
    }

    internal string Name { get; }

    public override string ToString() => Name;
}