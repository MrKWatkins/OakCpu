using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class Function
{
    private protected Function(string name, DataType type, IReadOnlyList<string> parameters, bool isTerminal = false)
    {
        Name = name;
        Type = type;
        Parameters = parameters;
        IsTerminal = isTerminal;
    }

    public string Name { get; }

    public IReadOnlyList<string> Parameters { get; }

    public DataType Type { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax();

    public bool IsTerminal { get; }

    public override string ToString() => $"{Name}";
}