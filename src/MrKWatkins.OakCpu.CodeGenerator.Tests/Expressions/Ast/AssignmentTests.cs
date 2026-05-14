using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Ast;

public sealed class AssignmentTests
{
    [Test]
    public void Constructor_AssignsTemporaryVariableType()
    {
        var temporary = new TemporaryVariable("temp");
        var target = new TemporaryVariableAccess(temporary);
        var assignment = new Assignment(target, new Number(1));

        assignment.Target.Should().Equal(target);
        assignment.Value.ToString().Should().Equal("0x01");
        temporary.Type.Should().Equal(DataType.U8);
    }

    [Test]
    public void Constructor_ThrowsForNonAccessTarget()
    {
        var exception = Assert.Throws<ArgumentException>(() => _ = new Assignment(new Number(1), new Number(2)));

        exception!.Message.Should().Contain("Value must be an Access");
    }

    [Test]
    public void Constructor_ThrowsForNonExpressionValue()
    {
        var exception = Assert.Throws<ArgumentException>(() => _ = new Assignment(new ArgumentAccess("value"), new TemporaryVariableDeclarationStatement(new TemporaryVariable("temp"))));

        exception!.Message.Should().Contain("Value must be an Expression");
    }

    [Test]
    public void ReplaceChild_ReplacesValue()
    {
        var assignment = new Assignment(new ArgumentAccess("value"), new Number(1));

        assignment.ReplaceChild(assignment.Value, new Number(2));

        assignment.Value.ToString().Should().Equal("0x02");
    }

    [Test]
    public void ReplaceChild_ThrowsForNonExpressionReplacement()
    {
        var assignment = new Assignment(new ArgumentAccess("value"), new Number(1));

        var exception = Assert.Throws<ArgumentException>(() => assignment.ReplaceChild(assignment.Value, new TemporaryVariableDeclarationStatement(new TemporaryVariable("temp"))));

        exception!.Message.Should().Contain("Value must be a Expression");
    }

    [Test]
    public void ReplaceChild_ThrowsForTarget()
    {
        var assignment = new Assignment(new ArgumentAccess("value"), new Number(1));

        var exception = Assert.Throws<InvalidOperationException>(() => assignment.ReplaceChild(assignment.Target, new ArgumentAccess("other")));

        exception!.Message.Should().Equal("Target cannot be replaced in an Assignment.");
    }

    [Test]
    public void ReplaceChild_ThrowsForUnknownNode()
    {
        var assignment = new Assignment(new ArgumentAccess("value"), new Number(1));

        var exception = Assert.Throws<ArgumentException>(() => assignment.ReplaceChild(new Number(3), new Number(2)));

        exception!.Message.Should().Contain("Value is not a child of this node.");
    }
}