using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Optimization;

public sealed class ConstantFoldingTests
{
    [TestCaseSource(nameof(BinaryConstantFoldCases))]
    public void Optimize_FoldsBinaryConstants(Operator @operator, int left, int right, string expected)
    {
        var operation = new BinaryOperation(@operator, new Number(left), new Number(right));

        var optimized = Optimizer.Optimize<AstNode>(operation);

        optimized.ToString().Should().Equal(expected);
    }

    [Test]
    public void Optimize_LeavesUnsupportedBinaryConstantOperation()
    {
        var operation = new BinaryOperation(Operator.Equality, new Number(1), new Number(2));

        var optimized = Optimizer.Optimize<AstNode>(operation);

        optimized.ToString().Should().Equal("0x01 == 0x02");
    }

    [Test]
    public void Optimize_FoldsUnaryNumberConstant()
    {
        var operation = new UnaryOperation(Operator.BitwiseNot, new Number(0x0F));

        var optimized = Optimizer.Optimize<AstNode>(operation);

        optimized.ToString().Should().Equal("0xF0");
    }

    [Test]
    public void Optimize_FoldsUnaryBooleanConstant()
    {
        var operation = new UnaryOperation(Operator.Not, Language.Ast.Boolean.True);

        var optimized = Optimizer.Optimize<AstNode>(operation);

        optimized.ToString().Should().Equal("False");
    }

    [Test]
    public void Optimize_LeavesUnaryOperationForNonConstantExpression()
    {
        var operation = new UnaryOperation(Operator.Not, new ArgumentAccess("R"));

        var optimized = Optimizer.Optimize<AstNode>(operation);

        optimized.ToString().Should().Equal("!(R)");
    }

    private static readonly object[] BinaryConstantFoldCases =
    [
        new object[] { Operator.Add, 1, 2, "0x03" },
        new object[] { Operator.Subtract, 5, 2, "0x03" },
        new object[] { Operator.LeftShift, 1, 2, "0x04" },
        new object[] { Operator.RightShift, 8, 1, "0x04" },
        new object[] { Operator.And, 3, 1, "0x01" },
        new object[] { Operator.Xor, 3, 1, "0x02" },
        new object[] { Operator.Or, 1, 2, "0x03" }
    ];
}