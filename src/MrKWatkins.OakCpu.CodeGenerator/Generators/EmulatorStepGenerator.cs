using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepGenerator : EmulatorClassGenerator
{
    private const string StepMethodName = "Step";
    public static readonly EmulatorStepGenerator Instance = new();

    private EmulatorStepGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(CreateStepMethod(input));

    [Pure]
    private static MethodDeclarationSyntax CreateStepMethod(GeneratorInput input) =>
        SyntaxFactory
            .MethodDeclaration(
                SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                SyntaxFactory.Identifier(StepMethodName))
            .AddModifiers(Public)
            .WithBody(SyntaxFactory.Block(
                CreateSwitch(input),
                CreateThrowNotSupportedException()));

    [Pure]
    private static SwitchStatementSyntax CreateSwitch(GeneratorInput input)
    {
        var sections = input.AllSteps.Select(CreateSwitchSection).ToArray();

        return SyntaxFactory
            .SwitchStatement(CreatePostIncrementExpression(KnownDataMember.Step.Name))
            .AddSections(sections);
    }

    [Pure]
    private static SwitchSectionSyntax CreateSwitchSection(Step step)
    {
        var statements = StatementGenerator.GenerateStatementSyntaxes(step.Statements).ToArray();

        return SyntaxFactory.SwitchSection()
            .AddLabels(SyntaxFactory.CaseSwitchLabel(GetNumericLiteralExpression(step.Index)))
            .AddStatements(statements)
            .WithLeadingTrivia(SyntaxFactory.Comment($"// {step.Name}"));
    }

    [Pure]
    private static ExpressionSyntax CreatePostIncrementExpression(string field) =>
        SyntaxFactory.PostfixUnaryExpression(
            SyntaxKind.PostIncrementExpression,
            SyntaxFactory.IdentifierName(field));

    [Pure]
    private static StatementSyntax CreateThrowNotSupportedException() =>
        SyntaxFactory.ThrowStatement(
                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("System.NotSupportedException"))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal($"The opcode 0x{{{KnownDataMember.Opcode.Name}:X2}} is not supported.")))))));
}