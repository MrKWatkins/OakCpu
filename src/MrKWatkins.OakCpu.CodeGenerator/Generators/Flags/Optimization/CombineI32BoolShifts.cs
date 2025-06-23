using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal sealed class CombineI32BoolShifts : FlagOptimization<I32BoolExpression>
{
    protected override I32BoolExpression Optimize(StatementGeneratorContext context, I32BoolExpression action)
    {
        var inlined = action.Expression.InlineUserDefinedFunctions(context);

        if (inlined is not BinaryOperation { Right: Number number } binaryOperation)
        {
            return action;
        }

        int shift;
        if (binaryOperation.Operator == Operator.LeftShift)
        {
            shift = number.Value;
        }
        else if (binaryOperation.Operator == Operator.RightShift)
        {
            shift = -number.Value;
        }
        else
        {
            return action;
        }

        return new I32BoolExpression(action, binaryOperation.Left, action.Shift + shift);
    }
}