namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal sealed class UnaryConstantFolding : Optimizer<UnaryOperation>
{
    protected override AstNode OptimizeNode(UnaryOperation unaryOperation) =>
        unaryOperation.Expression switch
        {
            Number number => OptimizeNumber(unaryOperation, number),
            Boolean boolean => OptimizeBoolean(unaryOperation, boolean),
            _ => unaryOperation
        };

    [Pure]
    private static Number OptimizeNumber(UnaryOperation unaryOperation, Number number) =>
        unaryOperation.Operator.Symbol switch
        {
            // Assuming we're a byte here as that's the only use case so far.
            "~" => new Number((byte)~number.Value),
            _ => number
        };

    [Pure]
    private static Boolean OptimizeBoolean(UnaryOperation unaryOperation, Boolean boolean) =>
        unaryOperation.Operator.Symbol switch
        {
            "!" => boolean == Boolean.True ? Boolean.False : Boolean.True,
            _ => boolean
        };
}