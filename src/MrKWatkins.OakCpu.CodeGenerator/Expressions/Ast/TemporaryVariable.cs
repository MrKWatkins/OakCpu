using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class TemporaryVariable
{
    public static readonly TemporaryVariable Byte = new("temp8", typeof(byte));

    public static readonly IReadOnlyDictionary<string, TemporaryVariable> All = new Dictionary<string, TemporaryVariable>(StringComparer.OrdinalIgnoreCase)
    {
        { Byte.Name, Byte }
    };

    private TemporaryVariable(string name, Type type)
    {
        Name = name;
        Type = type;

        if (Type == typeof(byte))
        {
            TypeSyntax = PredefinedType(Token(SyntaxKind.ByteKeyword));
        }
        else
        {
            throw new NotSupportedException($"The type {Type.Name} is not supported.");
        }
    }

    public string Name { get; }

    public Type Type { get; }

    public TypeSyntax TypeSyntax { get; }

    public override string ToString() => Name;
}