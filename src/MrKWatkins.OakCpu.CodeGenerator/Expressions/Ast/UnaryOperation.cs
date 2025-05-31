using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

/// <summary>
/// A unary operation.
/// </summary>
public sealed class UnaryOperation : Expression
{
    internal UnaryOperation(char @operator, AstNode expression)
    {
        Operator = @operator;
        Expression = expression as Expression ?? throw new ArgumentException($"Value must be a {nameof(Expression)}, not a {expression.GetType().Name}.", nameof(expression));
    }

    /// <summary>
    /// The operator.
    /// </summary>
    public char Operator { get; }

    public Expression Expression { get; }

    /// <summary>
    /// The <see cref="SyntaxKind" /> for the operator.
    /// </summary>
    public SyntaxKind OperatorSyntaxKind => Operator switch
    {
        '~' => SyntaxKind.BitwiseNotExpression,
        _ => throw new NotSupportedException($"The operator {Operator} is not supported.")
    };

    public override Type Type => typeof(int);

    public override TypeSyntax TypeSyntax => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append(Operator);
        expression.Append('(');
        Expression.WriteExpression(expression);
        expression.Append(')');
    }
}