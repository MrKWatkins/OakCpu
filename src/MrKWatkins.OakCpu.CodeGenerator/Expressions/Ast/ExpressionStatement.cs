using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class ExpressionStatement : Statement
{
    internal ExpressionStatement(Expression expression)
    {
        Expression = expression;
    }

    public Expression Expression { get; }

    public override void WriteExpression(StringBuilder expression) => Expression.WriteExpression(expression);
}