using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator;

public static class DataTypeExtensions
{
    [Pure]
    public static int Size(this DataType type) =>
        type switch
        {
            DataType.Void => 0,
            DataType.U8 => 1,
            DataType.I8 => 1,
            DataType.U16 => 2,
            DataType.I32 => 4,
            DataType.I32Bool => 4,
            DataType.Bool => 1,
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };

    [Pure]
    public static Type Type(this DataType type) =>
        type switch
        {
            DataType.Void => typeof(void),
            DataType.U8 => typeof(byte),
            DataType.I8 => typeof(sbyte),
            DataType.U16 => typeof(ushort),
            DataType.I32 => typeof(int),
            DataType.I32Bool => typeof(int),
            DataType.Bool => typeof(bool),
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };

    [Pure]
    public static PredefinedTypeSyntax TypeSyntax(this DataType type) =>
        type switch
        {
            DataType.Void => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
            DataType.U8 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
            DataType.I8 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword)),
            DataType.U16 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
            DataType.I32 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            DataType.I32Bool => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            DataType.Bool => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };
}