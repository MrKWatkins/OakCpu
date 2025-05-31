using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class DataMember
{
    public static readonly DataMember OpcodeStepTable = new("OpcodeStepTable", typeof(ushort[]));
    public static readonly DataMember Address = new("Address", typeof(ushort));
    public static readonly DataMember Data = new("Data", typeof(byte));
    public static readonly DataMember Opcode = new("opcode", typeof(byte));
    public static readonly DataMember Step = new("step", typeof(ushort));

    public static readonly IReadOnlyDictionary<string, DataMember> All = new Dictionary<string, DataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { OpcodeStepTable.Name, OpcodeStepTable },
        { Address.Name, Address },
        { Data.Name, Data },
        { Opcode.Name, Opcode },
        { Step.Name, Step }
    };

    private DataMember(string name, Type type)
    {
        Name = name;
        Type = type;

        if (Type == typeof(byte))
        {
            Size = 1;
            TypeSyntax = PredefinedType(Token(SyntaxKind.ByteKeyword));
        }
        else if (Type == typeof(ushort))
        {
            Size = 2;
            TypeSyntax = PredefinedType(Token(SyntaxKind.UShortKeyword));
        }
        else if (Type == typeof(ushort[]))
        {
            Size = 8;
            TypeSyntax = ArrayType(PredefinedType(Token(SyntaxKind.UShortKeyword)))
                .WithRankSpecifiers(
                    SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))));
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