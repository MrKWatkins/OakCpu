using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
        MethodDeclaration(
                IdentifierName(ActionRequiredEnumName),
                Identifier(StepMethodName))
            .AddModifiers(Public)
            .WithBody(Block(
                CreateSwitch(input),
                CreateThrowNotSupportedException()));

    [Pure]
    private static SwitchStatementSyntax CreateSwitch(GeneratorInput input)
    {
        var sections = input.AllSteps.Select(CreateSwitchSection).ToArray();

        return SwitchStatement(CreatePostIncrementExpression(KnownDataMember.Step.Name))
            .AddSections(sections);
    }

    [Pure]
    private static SwitchSectionSyntax CreateSwitchSection(Step step)
    {
        var statements = StatementGenerator.GenerateStatementSyntaxes(step.Statements).ToArray();

        return SwitchSection()
            .AddLabels(CaseSwitchLabel(GetNumericLiteralExpression(step.Index)))
            .AddStatements(statements)
            .WithLeadingTrivia(Comment($"// {step.Name}"));
    }

    [Pure]
    private static ExpressionSyntax CreatePostIncrementExpression(string field) =>
        PostfixUnaryExpression(
            SyntaxKind.PostIncrementExpression,
            IdentifierName(field));

    [Pure]
    private static StatementSyntax CreateThrowNotSupportedException() =>
        ThrowStatement(
                ObjectCreationExpression(IdentifierName("System.NotSupportedException"))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal($"The opcode 0x{{{KnownDataMember.Opcode.Name}:X2}} is not supported.")))))));
}