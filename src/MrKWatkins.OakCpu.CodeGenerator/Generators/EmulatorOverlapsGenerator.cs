using System.Runtime.CompilerServices;
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

public sealed class EmulatorOverlapsGenerator : EmulatorClassGenerator
{
    private const string SerializeOverlapPipelineMethodName = "SerializeOverlapPipeline";
    private const string RestoreOverlapPipelineMethodName = "RestoreOverlapPipeline";

    public static readonly EmulatorOverlapsGenerator Instance = new();

    private EmulatorOverlapsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.Emulator(context)}.overlaps";

    protected override ClassDeclarationSyntax PopulateClass(FileGeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .AddMembers(CreateExecuteOverlapMethod(context), CreateSerializeOverlapPipelineMethod(), CreateRestoreOverlapPipelineMethod())
            .AddMembers(context.GeneratorContext.OverlapSteps.Select(step => CreateOverlapMethod(context, step)).ToArray());

    [Pure]
    private static MemberDeclarationSyntax CreateOverlapMethod(FileGeneratorContext context, Step step)
    {
        var statements = StatementGenerator.GenerateOverlapStatements(context, step);

        var comments = context.GeneratorContext.GetOverlapImplementationAndDuplicates(step).Select(s => Comment($"// Overlap {s.Name}"));

        var function = MethodDeclaration(VoidType, Identifier(Method.Name.Overlap(context, step)))
            .WithModifiers([Private, Static])
            .WithParameterList(ParameterList(
            [
                Parameter.Syntax.Emulator(context)
            ]))
            .WithBody(Block(statements));

        return step == context.GeneratorContext.OpcodeRead.FirstStep.Implementation
            ? function.WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)]).WithLeadingTrivia(comments)])
            : function.WithLeadingTrivia(comments);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateExecuteOverlapMethod(FileGeneratorContext context)
    {
        const string overlapVariableName = "overlap";

        return MethodDeclaration(VoidType, Identifier(Method.Name.ExecuteOverlap))
            .WithModifiers([Private])
            .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)])])
            .WithBody(Block(
                IfStatement(
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName),
                        LiteralExpression(SyntaxKind.DefaultLiteralExpression)),
                    Block(ReturnStatement())),

                LocalDeclarationStatement(
                    VariableDeclaration(CreateOverlapHandlerType(context))
                        .WithVariables([
                            VariableDeclarator(overlapVariableName)
                                .WithInitializer(
                                    EqualsValueClause(IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName)))
                        ])),

                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName),
                        LiteralExpression(SyntaxKind.DefaultLiteralExpression))),

                ExpressionStatement(
                    InvocationExpression(IdentifierName(overlapVariableName))
                        .WithArgumentList(ArgumentList([Argument(ThisExpression())])))));
    }

    [Pure]
    private static MemberDeclarationSyntax CreateSerializeOverlapPipelineMethod()
    {
        const string indexVariableName = "index";

        return MethodDeclaration(UShortType, Identifier(SerializeOverlapPipelineMethodName))
            .WithModifiers([Private])
            .WithBody(Block(
                IfStatement(
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName),
                        LiteralExpression(SyntaxKind.DefaultLiteralExpression)),
                    Block(ReturnStatement(GenerateNumericLiteralExpression(0)))),

                ForStatement(
                        Block(
                            IfStatement(
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    CastExpression(
                                        IdentifierName("nuint"),
                                        ElementAccessExpression(IdentifierName(Field.Name.Overlaps))
                                            .WithArgumentList(BracketedArgumentList([Argument(IdentifierName(indexVariableName))]))),
                                    CastExpression(
                                        IdentifierName("nuint"),
                                        IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName))),
                                Block(ReturnStatement(CastExpression(UShortType, IdentifierName(indexVariableName)))))))
                    .WithDeclaration(
                        VariableDeclaration(IdentifierName("var"))
                            .WithVariables([VariableDeclarator(indexVariableName).WithInitializer(EqualsValueClause(GenerateNumericLiteralExpression(1)))]))
                    .WithCondition(
                        BinaryExpression(
                            SyntaxKind.LessThanExpression,
                            IdentifierName(indexVariableName),
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Field.Name.Overlaps), IdentifierName(nameof(Array.Length)))))
                    .WithIncrementors([
                        PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, IdentifierName(indexVariableName))
                    ]),

                ThrowStatement(
                    ObjectCreationExpression(IdentifierName(nameof(InvalidOperationException)))
                        .WithArgumentList(ArgumentList([Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Unknown overlap pipeline.")))])))));
    }

    [Pure]
    private static MemberDeclarationSyntax CreateRestoreOverlapPipelineMethod() =>
        MethodDeclaration(VoidType, Identifier(RestoreOverlapPipelineMethodName))
            .WithModifiers([Private])
            .WithParameterList(ParameterList([
                Parameter(Identifier("overlapIndex")).WithType(UShortType)
            ]))
            .WithBody(Block(
                IfStatement(
                    BinaryExpression(
                        SyntaxKind.GreaterThanOrEqualExpression,
                        IdentifierName("overlapIndex"),
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Field.Name.Overlaps), IdentifierName(nameof(Array.Length)))),
                    Block(
                        ThrowStatement(
                            ObjectCreationExpression(IdentifierName(nameof(InvalidOperationException)))
                                .WithArgumentList(ArgumentList([Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Unknown overlap pipeline.")))]))))),
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName),
                        ElementAccessExpression(IdentifierName(Field.Name.Overlaps))
                            .WithArgumentList(BracketedArgumentList([Argument(IdentifierName("overlapIndex"))]))))));
}