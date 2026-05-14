using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

internal sealed class BoolExpression : FlagAction
{
    private readonly Expression originalExpression;

    private BoolExpression(Flag flag, Expression expression, bool bitCastFromBoolToByte)
        : this([flag], expression, expression, flag.Index, bitCastFromBoolToByte)
    {
    }

    private BoolExpression(IReadOnlyList<Flag> flags, Expression originalExpression, Expression expression, int shift, bool bitCastFromBoolToByte)
        : base(flags)
    {
        this.originalExpression = originalExpression;
        Expression = expression;
        Shift = shift;
        BitCastFromBoolToByte = bitCastFromBoolToByte;
    }

    internal BoolExpression(BoolExpression original, Expression expression, int shift)
        : this(original.Flags, original.originalExpression, expression, shift, false)
    {
    }

    internal Expression Expression { get; }

    internal int Shift { get; }

    internal bool BitCastFromBoolToByte { get; }

    internal override int Order => 2;

    internal override ExpressionSyntax GenerateExpression(StatementGeneratorContext context)
    {
        var expressionSyntax = ExpressionGenerator.GenerateExpressionSyntax(context, Expression);
        if (BitCastFromBoolToByte)
        {
            expressionSyntax = GenerateBitCastFromBoolToByte(context, expressionSyntax);
        }

        return Shift switch
        {
            > 0 => BinaryExpression(SyntaxKind.LeftShiftExpression, ParenthesizedExpression(expressionSyntax), GenerateNumericLiteralExpression(Shift)),
            < 0 => BinaryExpression(SyntaxKind.RightShiftExpression, ParenthesizedExpression(expressionSyntax), GenerateNumericLiteralExpression(-Shift)),
            _ => expressionSyntax
        };
    }

    [MustUseReturnValue]
    private static ExpressionSyntax GenerateBitCastFromBoolToByte(StatementGeneratorContext context, ExpressionSyntax value)
    {
        context.RequiredUsings.Add(typeof(Unsafe));

        return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("Unsafe"),
                    GenericName(Identifier("BitCast"))
                        .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>([BoolType, ByteType])))))
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(value))));
    }

    internal override string GenerateComment() => $"// Set {FlagsNames(Flags)} if {originalExpression} is true.";

    [Pure]
    internal static FlagAction? CreateOrNull(Flag flag, Expression expression) => expression.Type switch
    {
        DataType.Bool => new BoolExpression(flag, expression, true),
        DataType.I32Bool => new BoolExpression(flag, expression, false),
        _ => null
    };
}