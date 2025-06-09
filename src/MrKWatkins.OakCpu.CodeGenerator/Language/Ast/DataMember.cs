using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class DataMember
{
    public static readonly DataMember OpcodeStepTable = new("opcodeStepTable", DataType.U16, typeof(ushort[]));
    public static readonly DataMember Address = new("Address", DataType.U16, typeof(ushort));
    public static readonly DataMember Data = new("Data", DataType.U8, typeof(byte));
    public static readonly DataMember Latch = new("latch", DataType.U8, typeof(byte));
    public static readonly DataMember Step = new("step", DataType.U16, typeof(ushort));

    public static readonly IReadOnlyDictionary<string, DataMember> All = new Dictionary<string, DataMember>(StringComparer.OrdinalIgnoreCase)
    {
        { OpcodeStepTable.Name, OpcodeStepTable },
        { Address.Name, Address },
        { Data.Name, Data },
        { Latch.Name, Latch },
        { Step.Name, Step }
    };

    private DataMember(string name, DataType type, Type memberType)
    {
        Name = name;
        Type = type;
        MemberType = memberType;

        if (MemberType == typeof(byte))
        {
            MemberSize = 1;
            MemberTypeSyntax = PredefinedType(Token(SyntaxKind.ByteKeyword));
        }
        else if (MemberType == typeof(ushort))
        {
            MemberSize = 2;
            MemberTypeSyntax = PredefinedType(Token(SyntaxKind.UShortKeyword));
        }
        else if (MemberType == typeof(ushort[]))
        {
            MemberSize = 8;

            var ushortType = PredefinedType(Token(SyntaxKind.UShortKeyword));
            MemberTypeSyntax = ArrayType(ushortType).WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]);
        }
        else
        {
            throw new NotSupportedException($"The type {MemberType.Name} is not supported.");
        }
    }

    public string Name { get; }

    public DataType Type { get; }

    public Type MemberType { get; }

    public TypeSyntax MemberTypeSyntax { get; }

    public int MemberSize { get; }
}