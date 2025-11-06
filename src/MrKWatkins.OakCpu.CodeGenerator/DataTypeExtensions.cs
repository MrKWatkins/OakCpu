using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

public static class DataTypeExtensions
{
    extension(DataType type)
    {
        [Pure]
        public int Size(bool isArray = false)
        {
            // Arrays are references, so they are 64 bits.
            if (isArray)
            {
                return 8;
            }

            return type switch
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
        }

        [Pure]
        public TypeSyntax TypeSyntax(bool isArray = false)
        {
            var typeSyntax = type switch
            {
                DataType.Void => PredefinedType(Token(SyntaxKind.VoidKeyword)),
                DataType.U8 => PredefinedType(Token(SyntaxKind.ByteKeyword)),
                DataType.I8 => PredefinedType(Token(SyntaxKind.SByteKeyword)),
                DataType.U16 => PredefinedType(Token(SyntaxKind.UShortKeyword)),
                DataType.I32 => PredefinedType(Token(SyntaxKind.IntKeyword)),
                DataType.I32Bool => PredefinedType(Token(SyntaxKind.IntKeyword)),
                DataType.Bool => PredefinedType(Token(SyntaxKind.BoolKeyword)),
                _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
            };

            return isArray
                ? ArrayType(typeSyntax).WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])])
                : typeSyntax;
        }

        [Pure]
        public bool IsSigned => type switch
        {
            DataType.U8 => false,
            DataType.I8 => true,
            DataType.U16 => false,
            DataType.I32 => true,
            DataType.I32Bool => true,
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not numeric.")
        };
    }
}