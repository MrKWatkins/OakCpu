using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public static class ExpressionExtensions
{
    /// <summary>
    /// Replaces calls to user defined functions in the expression with the function's expression, substituting its arguments.
    /// </summary>
    [Pure]
    public static Expression InlineUserDefinedFunctions(this Expression expression, StatementGeneratorContext context)
    {
        switch (expression)
        {
            case ArgumentAccess argumentAccess:
                return context.ArgumentScope[argumentAccess.Name];

            case BinaryOperation binaryOperation:
                var left = InlineUserDefinedFunctions(binaryOperation.Left, context);
                var right = InlineUserDefinedFunctions(binaryOperation.Right, context);
                if (!ReferenceEquals(left, binaryOperation.Left) || !ReferenceEquals(right, binaryOperation.Right))
                {
                    return new BinaryOperation(binaryOperation.Operator, InlineUserDefinedFunctions(binaryOperation.Left, context), InlineUserDefinedFunctions(binaryOperation.Right, context));
                }
                break;

            case Call call:
                context = context.WithArguments(call.Function.Parameters, call.Arguments);
                if (call.Function is UserDefinedFunction function)
                {
                    return InlineUserDefinedFunctions(function.Expression, context);
                }

                var inlinedArguments = call.Arguments.Select(a => InlineUserDefinedFunctions(a, context)).ToList();
                if (!inlinedArguments.SequenceEqual(call.Arguments))
                {
                    return new Call(call.Function, inlinedArguments);
                }
                break;

            case UnaryOperation unaryOperation:
                var unary = InlineUserDefinedFunctions(unaryOperation.Expression, context);
                if (!ReferenceEquals(expression, unaryOperation.Expression))
                {
                    return new UnaryOperation(unaryOperation.Operator, unary);
                }
                break;
        }

        return expression;
    }
}