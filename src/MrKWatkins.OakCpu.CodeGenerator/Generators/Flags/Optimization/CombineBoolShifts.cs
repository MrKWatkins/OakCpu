using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal sealed class CombineBoolShifts : FlagOptimization<BoolExpression>
{
    protected override FlagAction Optimize(StatementGeneratorContext context, BoolExpression action)
    {
        if (action.BitCastFromBoolToByte)
        {
            return action;
        }

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

        return new BoolExpression(action, binaryOperation.Left, action.Shift + shift);
    }
}