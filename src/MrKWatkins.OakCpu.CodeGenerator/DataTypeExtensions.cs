using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator;

/// <summary>
/// Provides Roslyn and serialization helpers for generator data types.
/// </summary>
public static class DataTypeExtensions
{
    extension(DataType type)
    {
        /// <summary>
        /// Gets the size in bytes for the data type.
        /// </summary>
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

        /// <summary>
        /// Creates the C# type syntax for the data type.
        /// </summary>
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

        /// <summary>
        /// Gets whether the data type is signed.
        /// </summary>
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

        /// <summary>
        /// Creates the default C# literal expression for the data type.
        /// </summary>
        [Pure]
        public ExpressionSyntax DefaultLiteral() => type switch
        {
            DataType.U8 => GenerateNumericLiteralExpression(0),
            DataType.I8 => GenerateNumericLiteralExpression(0),
            DataType.U16 => GenerateNumericLiteralExpression(0),
            DataType.I32 => GenerateNumericLiteralExpression(0),
            DataType.I32Bool => GenerateNumericLiteralExpression(0),
            DataType.Bool => LiteralExpression(SyntaxKind.FalseLiteralExpression),
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };

        /// <summary>
        /// Gets the <see cref="BinaryReader"/> method name used to deserialize the data type.
        /// </summary>
        [Pure]
        public string BinaryReaderMethodName() => type switch
        {
            DataType.U8 => nameof(BinaryReader.ReadByte),
            DataType.I8 => nameof(BinaryReader.ReadSByte),
            DataType.U16 => nameof(BinaryReader.ReadUInt16),
            DataType.I32 => nameof(BinaryReader.ReadInt32),
            DataType.I32Bool => nameof(BinaryReader.ReadInt32),
            DataType.Bool => nameof(BinaryReader.ReadBoolean),
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
        };
    }
}