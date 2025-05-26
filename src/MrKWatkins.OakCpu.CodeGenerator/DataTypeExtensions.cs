using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator;

public static class DataTypeExtensions
{
    [Pure]
    public static int Size(this DataType type) =>
        type switch
        {
            DataType.U8 => 1,
            DataType.U16 => 2,
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };

    [Pure]
    public static Type Type(this DataType type) =>
        type switch
        {
            DataType.U8 => typeof(byte),
            DataType.U16 => typeof(ushort),
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };

    [Pure]
    public static PredefinedTypeSyntax TypeSyntax(this DataType type) =>
        type switch
        {
            DataType.U8 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
            DataType.U16 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };
}