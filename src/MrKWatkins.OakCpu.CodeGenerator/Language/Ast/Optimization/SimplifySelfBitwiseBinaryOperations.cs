namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal sealed class SimplifySelfBitwiseBinaryOperations : Optimizer<BinaryOperation>
{
    protected override AstNode OptimizeNode(BinaryOperation binaryOperation)
    {
        if (binaryOperation.Left == binaryOperation.Right)
        {
            return binaryOperation.Operator.Symbol switch
            {
                "^" => new Number(0),
                "&" => binaryOperation.Left,
                "|" => binaryOperation.Left,
                _ => binaryOperation
            };
        }
        return binaryOperation;
    }
}