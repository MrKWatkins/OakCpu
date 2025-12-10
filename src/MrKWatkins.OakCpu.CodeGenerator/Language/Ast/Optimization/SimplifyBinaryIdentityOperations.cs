namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal sealed class SimplifyBinaryIdentityOperations : Optimizer<BinaryOperation>
{
    protected override AstNode OptimizeNode(BinaryOperation binaryOperation)
    {
        // If one operand is the identity value for that operator, we can simplify certain operations.
        if (binaryOperation.Operator.LeftIdentity.HasValue &&
            binaryOperation.Left is Number leftNumber &&
            leftNumber.Value == binaryOperation.Operator.LeftIdentity.Value)
        {
            return binaryOperation.Right;
        }
        if (binaryOperation.Operator.RightIdentity.HasValue &&
            binaryOperation.Right is Number rightNumber &&
            rightNumber.Value == binaryOperation.Operator.RightIdentity.Value)
        {
            return binaryOperation.Left;
        }

        return binaryOperation;
    }
}