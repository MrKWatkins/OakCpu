using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class Number(int value) : Expression
{
    public int Value { get; } = value;

    public override Type Type =>
        Value switch
        {
            <= 255 => typeof(byte),
            <= 65535 => typeof(ushort),
            _ => typeof(int)
        };

    public override TypeSyntax TypeSyntax =>
        Value switch
        {
            <= 255 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
            <= 65535 => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
            _ => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))
        };

    public string NumberString =>
        Value switch
        {
            <= 255 => $"0x{Value:X2}",
            <= 65535 => $"0x{Value:X4}",
            _ => Value.ToString()
        };

    public override void WriteExpression(StringBuilder expression) => expression.Append(NumberString);
}