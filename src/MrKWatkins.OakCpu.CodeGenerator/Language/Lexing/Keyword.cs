using System.Collections.Frozen;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Lexing;

internal sealed record Keyword : Token
{
    public const string If = "if";
    public const string Else = "else";
    public const string EndIf = "endif";

    public static readonly FrozenSet<string> All = new HashSet<string> { If, Else, EndIf }.ToFrozenSet();

    internal Keyword(int index, string name)
        : base(index, name.Length)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;
}