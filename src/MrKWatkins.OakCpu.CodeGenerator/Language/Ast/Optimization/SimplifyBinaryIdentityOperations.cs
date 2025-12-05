namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal sealed class SimplifyBinaryIdentityOperations : Optimizer<BinaryOperation>
{
    protected override AstNode OptimizeNode(BinaryOperation unaryOperation)
    {
        // If one operand is the identity value for that operator, we can simplify certain operations.
        if (unaryOperation.Operator.LeftIdentity.HasValue &&
            unaryOperation.Left is Number leftNumber &&
            leftNumber.Value == unaryOperation.Operator.LeftIdentity.Value)
        {
            return unaryOperation.Right;
        }
        if (unaryOperation.Operator.RightIdentity.HasValue &&
            unaryOperation.Right is Number rightNumber &&
            rightNumber.Value == unaryOperation.Operator.RightIdentity.Value)
        {
            return unaryOperation.Left;
        }

        return unaryOperation;
    }
}