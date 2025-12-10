namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal sealed class BinaryConstantFolding : Optimizer<BinaryOperation>
{
    protected override AstNode OptimizeNode(BinaryOperation binaryOperation)
    {
        // Only worrying about numbers for now.
        // If one operand is the identity value for that operator, we can simplify certain operations.
        if (binaryOperation.Left is not Number leftNumber ||
            binaryOperation.Right is not Number rightNumber)
        {
            return binaryOperation;
        }

        int? value = binaryOperation.Operator.Symbol switch
        {
            "+" => leftNumber.Value + rightNumber.Value,
            "-" => leftNumber.Value - rightNumber.Value,
            "<<" => leftNumber.Value << rightNumber.Value,
            ">>" => leftNumber.Value >> rightNumber.Value,
            "&" => leftNumber.Value & rightNumber.Value,
            "^" => leftNumber.Value ^ rightNumber.Value,
            "|" => leftNumber.Value | rightNumber.Value,
            _ => null
        };

        return value is not null ? new Number(value.Value) : binaryOperation;
    }
}