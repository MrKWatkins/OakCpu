using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepsGenerator : EmulatorClassGenerator
{
    private const string StepMethodName = "Step";

    public static readonly EmulatorStepsGenerator Instance = new();

    private EmulatorStepsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetEmulatorClassName(context)}.steps";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .AddMembers(CreateStepMethod(context), CreateErrorFunction(context))
            .AddMembers(context.AllSteps.Where(s => !s.DoesNothing).Select(step => CreateStepFunction(context, step)).ToArray());

    [Pure]
    private static MemberDeclarationSyntax CreateErrorFunction(GeneratorContext context)
    {
        var throwStatement = ThrowStatement(
            ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                .WithArgumentList(
                    ArgumentList([Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Opcode not supported")))])));

        return CreateFunction(context, ErrorFunctionName, [throwStatement]);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateStepFunction(GeneratorContext context, Step step)
    {
        var statements = StatementGenerator.GenerateStatements(context, step);

        var comment = Comment($"// {step.Name}");

        var function = CreateFunction(context, GetStepFunctionName(step), statements);

        // Aggressively inline step 0 as it is called for overlapped reads.
        function = step == context.OpcodeRead.FirstStep
            ? function.WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)]).WithLeadingTrivia(comment)])
            : function.WithLeadingTrivia(comment);

        return function;
    }

    [Pure]
    private static MemberDeclarationSyntax CreateFunction(GeneratorContext context, string name, IEnumerable<StatementSyntax> statements) =>
        MethodDeclaration(VoidType, Identifier(name))
            .WithModifiers([Private, Static])
            .WithParameterList(ParameterList(
            [
                CreateEmulatorParameter(context),
                Parameter(Identifier(ActionRequiredParameterName)).WithType(IdentifierName(ActionRequiredEnumName)).WithModifiers([Ref])
            ]))
            .WithBody(Block(statements));

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
                                    EqualsValueClause(CreateArrayGetWithoutBoundsCheck(context.RequiredUsings, IdentifierName(StepsFieldName), IdentifierName(PreDefinedDataMember.CurrentStep.FieldName))))
                        ])),

                // currentStep = node.NextStep;
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(PreDefinedDataMember.CurrentStep.FieldName),
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(StepNextStepFieldName)))),

                // var actionRequired = node.ActionRequired;
                LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName("var"))
                        .WithVariables([
                            VariableDeclarator(ActionRequiredParameterName)
                                .WithInitializer(
                                    EqualsValueClause(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(ActionRequiredEnumName))))
                        ])),

                // if (step.Handler != default)
                // {
                //     step.Handler(this, ref actionRequired);
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
                                .WithArgumentList(ArgumentList(
                                    [
                                        Argument(ThisExpression()),
                                        Argument(RefExpression(IdentifierName(ActionRequiredParameterName)))
                                    ]))))),

                // return node.ActionRequired;
                ReturnStatement(IdentifierName(ActionRequiredParameterName))));
    }
}