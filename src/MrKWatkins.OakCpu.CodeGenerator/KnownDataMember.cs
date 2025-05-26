using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator;

public sealed class KnownDataMember
{
    public static readonly KnownDataMember Address = new("Address", typeof(ushort), false);
    public static readonly KnownDataMember Data = new("Data", typeof(byte), false);
    public static readonly KnownDataMember Opcode = new("opcode", typeof(byte), true);
    public static readonly KnownDataMember Step = new("step", typeof(ushort), true);

    public static readonly IReadOnlyDictionary<string, KnownDataMember> All = new Dictionary<string, KnownDataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { Address.Name, Address },
        { Data.Name, Data },
        { Opcode.Name, Opcode },
        { Step.Name, Step }
    };

    private KnownDataMember(string name, Type type, bool isField)
    {
        Name = name;
        Type = type;
        IsField = isField;

        if (Type == typeof(byte))
        {
            Size = 1;
            TypeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword));
        }
        else if (Type == typeof(ushort))
        {
            Size = 2;
            TypeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword));
        }
        else
        {
            throw new NotSupportedException($"The type {Type.Name} is not supported.");
        }
    }

    public string Name { get; }

    public Type Type { get; }

    public PredefinedTypeSyntax TypeSyntax { get; }

    public int Size { get; }

    public bool IsField { get; }

    public bool IsProperty => !IsField;
}