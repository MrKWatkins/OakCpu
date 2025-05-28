using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator;

public sealed class KnownDataMember
{
    public static readonly KnownDataMember OpcodeStepTable = new("OpcodeStepTable", typeof(ushort[]));
    public static readonly KnownDataMember Address = new("Address", typeof(ushort));
    public static readonly KnownDataMember Data = new("Data", typeof(byte));
    public static readonly KnownDataMember Opcode = new("opcode", typeof(byte));
    public static readonly KnownDataMember Step = new("step", typeof(ushort));

    public static readonly IReadOnlyDictionary<string, KnownDataMember> All = new Dictionary<string, KnownDataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { OpcodeStepTable.Name, OpcodeStepTable },
        { Address.Name, Address },
        { Data.Name, Data },
        { Opcode.Name, Opcode },
        { Step.Name, Step }
    };

    private KnownDataMember(string name, Type type)
    {
        Name = name;
        Type = type;

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
        else if (Type == typeof(ushort[]))
        {
            Size = 8;
            TypeSyntax = SyntaxFactory
                .ArrayType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)))
                .WithRankSpecifiers(
                    SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression()))));
        }
        else
        {
            throw new NotSupportedException($"The type {Type.Name} is not supported.");
        }
    }

    public string Name { get; }

    public Type Type { get; }

    public TypeSyntax TypeSyntax { get; }

    public int Size { get; }
}