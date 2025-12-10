using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class StatementGeneratorContextTests : TestFixture
{
    [Test]
    public void Constructor_WithoutStep()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, null);
        context.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        context.Step.Should().BeNull();
        context.ArgumentScope.Should().BeEmpty();
        context.InitializedTemporaryVariables.Should().BeEmpty();
        context.InBooleanContext.Should().BeFalse();
        context.Parent.Should().BeNull();
    }

    [Test]
    public void Constructor_WithStep()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, Step);
        context.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        context.Step.Should().BeTheSameInstanceAs(Step);
        context.ArgumentScope.Should().BeEmpty();
        context.InitializedTemporaryVariables.Should().BeEmpty();
        context.InBooleanContext.Should().BeFalse();
        context.Parent.Should().BeNull();
    }

    [Test]
    public void WithArguments()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, Step);

        var parameters = new[] { "x", "y" };
        var arguments = new[] { CreateExpression("5"), CreateExpression("true") };

        var newContext = context.WithArguments(parameters, arguments);
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.InitializedTemporaryVariables.Should().BeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.InBooleanContext.Should().BeFalse();
        newContext.Parent.Should().BeNull();

        newContext.ArgumentScope.Should().HaveCount(2);
        newContext.ArgumentScope["x"].Should().BeTheSameInstanceAs(arguments[0]);
        newContext.ArgumentScope["y"].Should().BeTheSameInstanceAs(arguments[1]);
    }

    [Test]
    public void WithBooleanContext()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, Step);

        var newContext = context.WithBooleanContext();
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.ArgumentScope.Should().BeTheSameInstanceAs(context.ArgumentScope);
        newContext.InitializedTemporaryVariables.Should().BeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.Parent.Should().BeNull();

        newContext.InBooleanContext.Should().BeTrue();
    }

    [Test]
    public void WithChildVariableScope()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, Step);
        context.InitializedTemporaryVariables.Add("test");

        var newContext = context.WithChildVariableScope();
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.ArgumentScope.Should().BeTheSameInstanceAs(context.ArgumentScope);
        newContext.InBooleanContext.Should().BeFalse();
        newContext.Parent.Should().BeNull();

        newContext.InitializedTemporaryVariables.Should().NotBeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.InitializedTemporaryVariables.Should().HaveCount(1);
        newContext.InitializedTemporaryVariables.Contains("test").Should().BeTrue();
    }

    [Test]
    public void WithParentExpression()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, Step);

        var parent = CreateExpression("10");

        var newContext = context.WithParentExpression(parent);
        newContext.GeneratorContext.Should().BeTheSameInstanceAs(Z80GeneratorContext);
        newContext.Step.Should().BeTheSameInstanceAs(Step);
        newContext.ArgumentScope.Should().BeTheSameInstanceAs(context.ArgumentScope);
        newContext.InitializedTemporaryVariables.Should().BeTheSameInstanceAs(context.InitializedTemporaryVariables);
        newContext.InBooleanContext.Should().BeFalse();

        newContext.Parent.Should().BeTheSameInstanceAs(parent);
    }

    [Pure]
    private static Expression CreateExpression(string text)
    {
        var context = new ParserContext(Z80GeneratorContext.Configuration);
        return Parser.ParseExpression(context, text);
    }

    private static Step Step => Z80GeneratorContext.Instructions[0].FirstStep;
}