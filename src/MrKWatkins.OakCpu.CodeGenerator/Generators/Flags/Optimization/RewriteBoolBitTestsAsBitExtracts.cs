using System.Numerics;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

internal sealed class RewriteBoolBitTestsAsBitExtracts : FlagOptimization<BoolExpression>
{
    protected override FlagAction Optimize(StatementGeneratorContext context, BoolExpression action)
    {
        if (!action.BitCastFromBoolToByte)
        {
            return action;
        }

        var inlined = action.Expression.InlineUserDefinedFunctions(context);
        return TryGetBitExtract(inlined, out var expression, out var extractedBitIndex)
            ? new BitExtractExpression(action.Flags[0], action.Expression, expression, extractedBitIndex)
            : action;
    }

    [Pure]
    private static bool TryGetBitExtract(Expression expression, out Expression extractedExpression, out int extractedBitIndex)
    {
        if (expression is BinaryOperation { Operator: var op } equality && op == Operator.Equality)
        {
            return TryGetBitExtract(equality.Left, equality.Right, out extractedExpression, out extractedBitIndex) ||
                   TryGetBitExtract(equality.Right, equality.Left, out extractedExpression, out extractedBitIndex);
        }

        extractedExpression = null!;
        extractedBitIndex = 0;
        return false;
    }

    [Pure]
    private static bool TryGetBitExtract(Expression maskedExpression, Expression bitExpression, out Expression extractedExpression, out int extractedBitIndex)
    {
        if (bitExpression is not Number number || !IsSingleBitMask(number.Value))
        {
            extractedExpression = null!;
            extractedBitIndex = 0;
            return false;
        }

        if (maskedExpression is not BinaryOperation { Operator: var op } andExpression || op != Operator.And)
        {
            extractedExpression = null!;
            extractedBitIndex = 0;
            return false;
        }

        if (TryGetExtractedExpression(andExpression.Left, andExpression.Right, number.Value, out extractedExpression) ||
            TryGetExtractedExpression(andExpression.Right, andExpression.Left, number.Value, out extractedExpression))
        {
            extractedBitIndex = GetBitIndex(number.Value);
            return true;
        }

        extractedExpression = null!;
        extractedBitIndex = 0;
        return false;
    }

    [Pure]
    private static bool TryGetExtractedExpression(Expression candidateExpression, Expression candidateMask, int expectedMask, out Expression extractedExpression)
    {
        if (candidateMask is Number number && number.Value == expectedMask)
        {
            extractedExpression = candidateExpression;
            return true;
        }

        extractedExpression = null!;
        return false;
    }

    [Pure]
    private static bool IsSingleBitMask(int value) => value > 0 && (value & (value - 1)) == 0;

    [Pure]
    private static int GetBitIndex(int value) => BitOperations.TrailingZeroCount(value);
}