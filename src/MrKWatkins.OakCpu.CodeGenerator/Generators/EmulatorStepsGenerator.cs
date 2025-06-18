using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepsGenerator : EmulatorClassGenerator
{
    private const string StepMethodName = "Step";

    public static readonly EmulatorStepsGenerator Instance = new();

    private EmulatorStepsGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .AddMembers(CreateStepMethod(context))
            .AddMembers(context.AllSteps.Where(s => !s.DoesNothing).Select(step => CreateStepFunction(context, step)).ToArray());

    [Pure]
    private static MemberDeclarationSyntax CreateStepFunction(GeneratorContext context, Step step)
    {
        var statements = StepGenerator.GenerateStatements(context, step);

        return MethodDeclaration(Void, Identifier(GetStepFunctionName(step)))
            .WithModifiers([Private, Static])
            .WithParameterList(ParameterList([CreateEmulatorParameter(context)]))
            .WithBody(Block(statements))
            .WithLeadingTrivia(Comment($"// {step.Name}"));
    }

    [Pure]
    private static MethodDeclarationSyntax CreateStepMethod(GeneratorContext context)
    {
        const string stepVariableName = "step";

        return MethodDeclaration(
                IdentifierName(ActionRequiredEnumName),
                Identifier(StepMethodName))
            .AddModifiers(Public)
            .WithBody(Block(
                // var node = Nodes[currentStep];
                LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName("var"))
                        .WithVariables([
                            VariableDeclarator(stepVariableName)
                                .WithInitializer(
                                    EqualsValueClause(CreateArrayGetWithoutBoundsCheck(context, IdentifierName(StepsFieldName), IdentifierName(PreDefinedDataMember.CurrentStep.FieldName))))
                        ])),

                // currentStep = node.NextStep;
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(PreDefinedDataMember.CurrentStep.FieldName),
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(StepNextStepFieldName)))),

                // if (step.Handler != default)
                // {
                //     step.Handler(this);
                // }
                IfStatement(
                    BinaryExpression(
                        SyntaxKind.NotEqualsExpression,
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(StepHandlerFieldName)),
                        LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                    ),
                    Block(
                        ExpressionStatement(
                            InvocationExpression(
                                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(StepHandlerFieldName)))
                                .WithArgumentList(ArgumentList([Argument(ThisExpression())]))))),

                // return node.ActionRequired;
                ReturnStatement(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(stepVariableName),
                        IdentifierName(ActionRequiredEnumName)))));
    }
}