using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

// TODO: Could potentially combine these?
internal sealed class BoolExpression : FlagAction
{
    private BoolExpression(Flag flag, Expression expression)
        : base(flag)
    {
        Expression = expression;
    }

    public Expression Expression { get; }

    internal override int Order => 2;

    internal override ExpressionSyntax GenerateExpression(StatementGeneratorContext context)
    {
        var boolExpression = ExpressionGenerator.GenerateExpressionSyntax(context, Expression);

        // Benchmark suggests CMOV via a ternary should be twice as fast, but using it in the emulator made it slightly slower.
        // TODO: Revisit.
        // return ConditionalExpression(boolExpression, GenerateBinaryLiteralExpression(Flags[0].BitMask), GenerateBinaryLiteralExpression(0));
        var byteExpression = GenerateBitCastFromBoolToByte(context, boolExpression);

        var shift = Flags[0].Index;

        return shift != 0 ? BinaryExpression(SyntaxKind.LeftShiftExpression, ParenthesizedExpression(byteExpression), GenerateNumericLiteralExpression(shift)) : byteExpression;
    }

    private static ExpressionSyntax GenerateBitCastFromBoolToByte(StatementGeneratorContext context, ExpressionSyntax value)
    {
        context.GeneratorContext.RequiredUsings.Add("System.Runtime.CompilerServices");

        return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(Unsafe)),
                    GenericName(Identifier("BitCast"))
                        .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>([Bool, Byte])))))
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(value))));
    }


    internal override string GenerateComment(StatementGeneratorContext context) => $"// Set {FlagsNames(Flags)} if {Expression} is true.";

    [Pure]
    internal static FlagAction? CreateOrNull(Flag flag, Expression expression) =>
        expression.Type == DataType.Bool
            ? new BoolExpression(flag, expression)
            : null;
}