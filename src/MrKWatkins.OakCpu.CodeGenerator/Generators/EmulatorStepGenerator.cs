using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

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
                CreateActionVariable(),
                CreateSwitch(input),
                SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(ActionVariableName))));

    [Pure]
    private static SwitchStatementSyntax CreateSwitch(GeneratorInput input)
    {
        var sections = input.Cpu.OpcodeRead.Select((step, index) => CreateSwitchSection(index, step)).ToArray();

        return SyntaxFactory
            .SwitchStatement(CreatePostIncrementExpression(KnownDataMember.Step.Name))
            .AddSections(sections);
    }

    [Pure]
    private static SwitchSectionSyntax CreateSwitchSection(int index, Step step)
    {
        var statements = step.Expressions.Select(StatementGenerator.GenerateStatement).ToArray();

        return SyntaxFactory.SwitchSection()
            .AddLabels(SyntaxFactory.CaseSwitchLabel(GetNumericLiteralExpression(index)))
            .AddStatements(statements)
            .AddStatements(SyntaxFactory.BreakStatement());
    }

    [Pure]
    private static StatementSyntax CreateActionVariable() =>
        SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(ActionVariableName))
                        .WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                                    SyntaxFactory.IdentifierName(ActionRequiredNone)))))));

    [Pure]
    private static ExpressionSyntax CreatePostIncrementExpression(string field) =>
        SyntaxFactory.PostfixUnaryExpression(
            SyntaxKind.PostIncrementExpression,
            SyntaxFactory.IdentifierName(field));
}