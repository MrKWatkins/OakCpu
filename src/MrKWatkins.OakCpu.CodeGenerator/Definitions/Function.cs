using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class Function
{
    private protected Function(string name, Type type, IReadOnlyList<string> parameters)
    {
        Name = name;
        Type = type;
        Parameters = parameters;
        if (type == typeof(byte))
        {
            TypeSyntax = PredefinedType(Token(SyntaxKind.ByteKeyword));
        }
        else if (type == typeof(ushort))
        {
            TypeSyntax = PredefinedType(Token(SyntaxKind.UShortKeyword));
        }
        else if (type == typeof(int))
        {
            TypeSyntax = PredefinedType(Token(SyntaxKind.IntKeyword));
        }
        else if (type == typeof(void))
        {
            TypeSyntax = PredefinedType(Token(SyntaxKind.VoidKeyword));
        }
        else
        {
            throw new NotSupportedException($"The type {type.Name} is not supported.");
        }
    }

    public string Name { get; }

    public IReadOnlyList<string> Parameters { get; }

    public Type Type { get; }

    public TypeSyntax TypeSyntax { get; }

    public override string ToString() => $"{Name}";
}