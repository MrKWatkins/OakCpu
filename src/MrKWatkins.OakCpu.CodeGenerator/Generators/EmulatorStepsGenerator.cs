using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepsGenerator : EmulatorClassGenerator
{
    private const string StepMethodName = "Step";

    public static readonly EmulatorStepsGenerator Instance = new();

    private EmulatorStepsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.Emulator(context)}.steps";

    protected override ClassDeclarationSyntax PopulateClass(FileGeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .AddMembers(CreateStepMethod(context), CreateErrorFunction(context))
            .AddMembers(context.GeneratorContext.FunctionSteps.Where(step =>
            {
                var stepLayout = context.GeneratorContext.GetStepLayout(step);
                return stepLayout is { DoesNothing: false, ExecutesAsOverlapOnly: false };
            }).Select(step => CreateStepMethod(context, step)).ToArray());

    [Pure]
    private static MemberDeclarationSyntax CreateErrorFunction(FileGeneratorContext context)
    {
        var throwStatement = ThrowStatement(
            ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                .WithArgumentList(
                    ArgumentList([Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Opcode not supported")))])));

        return CreateFunction(context, Method.Name.Error, [throwStatement]);
    }

    [MustUseReturnValue]
    private static MemberDeclarationSyntax CreateStepMethod(FileGeneratorContext context, Step step)
    {
        var statements = StatementGenerator.GenerateStatements(context, step);

        var comments = context.GeneratorContext.GetStepLayout(step).ImplementationAndDuplicates.Select(s => Comment($"// {s.Name}"));

        var function = CreateFunction(context, Method.Name.Step(context.GeneratorContext, step), statements);

        // Aggressively inline step 0 as it is called for overlapped reads.
        function = step == context.GeneratorContext.OpcodeRead.FirstStep
            ? function.WithAttributeLists([CreateAggressiveInliningAttributeList(context.RequiredUsings).WithLeadingTrivia(comments)])
            : function.WithLeadingTrivia(comments);

        return function;
    }

    [Pure]
    private static MemberDeclarationSyntax CreateFunction(FileGeneratorContext context, string name, IEnumerable<StatementSyntax> statements) =>
        MethodDeclaration(VoidType, Identifier(name))
            .WithModifiers([Private, Static])
            .WithParameterList(ParameterList(
            [
                Parameter.Syntax.Emulator(context),
                Parameter(Identifier(Parameter.Name.ActionRequired)).WithType(IdentifierName(TypeName.ActionRequiredEnum)).WithModifiers([Ref])
            ]))
            .WithBody(Block(statements));

    [MustUseReturnValue]
    private static MethodDeclarationSyntax CreateStepMethod(FileGeneratorContext context)
    {
        const string stepVariableName = "step";
        var onStepCompleteStatements = StatementGenerator.GenerateStatements(context, context.GeneratorContext.OnStepComplete).ToArray();

        var stepMethodStatements = new List<StatementSyntax>
        {
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
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(Field.Name.NextStep)))),

            // var actionRequired = node.ActionRequired;
            LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                    .WithVariables([
                        VariableDeclarator(Parameter.Name.ActionRequired)
                            .WithInitializer(
                                EqualsValueClause(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(TypeName.ActionRequiredEnum))))
                    ])),

            // if (step.Handler != default)
            // {
            //     step.Handler(this, ref actionRequired);
            // }
            IfStatement(
                BinaryExpression(
                    SyntaxKind.NotEqualsExpression,
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(Field.Name.Handler)),
                    LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                ),
                Block(
                    ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(stepVariableName), IdentifierName(Field.Name.Handler)))
                            .WithArgumentList(ArgumentList(
                                [
                                    Argument(ThisExpression()),
                                    Argument(RefExpression(IdentifierName(Parameter.Name.ActionRequired)))
                                ]))))),
        };

        if (onStepCompleteStatements.Length != 0)
        {
            stepMethodStatements.Add(
                LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName("var"))
                        .WithVariables([
                            VariableDeclarator("emulator")
                                .WithInitializer(EqualsValueClause(ThisExpression()))
                        ])));

            stepMethodStatements.AddRange(onStepCompleteStatements);
        }

        stepMethodStatements.Add(ReturnStatement(IdentifierName(Parameter.Name.ActionRequired)));

        return WithXmlDocumentation(
            MethodDeclaration(
                    IdentifierName(TypeName.ActionRequiredEnum),
                    Identifier(StepMethodName))
                .AddModifiers(Public)
                .WithBody(Block(stepMethodStatements)),
            $"Executes one {context.GeneratorContext.Cpu.Name} T-state.",
            returns: "The external action that the host must perform for the completed T-state.");
    }

}