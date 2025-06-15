using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public abstract class Expression : AstNode, IEquatable<Expression>
{
    public abstract DataType Type { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax();

    public bool Equals(Expression? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ToString() == other.ToString();
    }

    public override bool Equals(object? obj) => Equals(obj as Expression);

    public override int GetHashCode() => ToString().GetHashCode();

    [Pure]
    public Expression InlineUserDefinedFunctions(StepContext context) => InlineUserDefinedFunctions(context, this);

    [Pure]
    private static Expression InlineUserDefinedFunctions(StepContext context, Expression expression)
    {
        switch (expression)
        {
            case ArgumentAccess argumentAccess:
                return context.ArgumentScope[argumentAccess.Name];

            case BinaryOperation binaryOperation:
                var left = InlineUserDefinedFunctions(context, binaryOperation.Left);
                var right = InlineUserDefinedFunctions(context, binaryOperation.Right);

                if (!ReferenceEquals(left, binaryOperation.Left) || !ReferenceEquals(right, binaryOperation.Right))
                {
                    return new BinaryOperation(binaryOperation.Operator, InlineUserDefinedFunctions(context, binaryOperation.Left), InlineUserDefinedFunctions(context, binaryOperation.Right));
                }

                break;

            case Call call:
                context = context.WithArguments(call.Function.Parameters, call.Arguments);
                if (call.Function is UserDefinedFunction function)
                {
                    return InlineUserDefinedFunctions(context, function.Expression);
                }

                var inlinedArguments = call.Arguments.Select(a => InlineUserDefinedFunctions(context, a)).ToList();
                if (!inlinedArguments.SequenceEqual(call.Arguments))
                {
                    return new Call(call.Function, inlinedArguments);
                }
                break;

            case UnaryOperation unaryOperation:
                var unary = InlineUserDefinedFunctions(context, unaryOperation.Expression);

                if (!ReferenceEquals(expression, unaryOperation.Expression))
                {
                    return new UnaryOperation(unaryOperation.Operator, unary);
                }

                break;
        }

        return expression;
    }
}