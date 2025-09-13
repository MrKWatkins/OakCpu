using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorInterruptsGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorInterruptsGenerator Instance = new();

    private EmulatorInterruptsGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .WithMembers(
            [
                CreateInterruptModesField(context),
                CreateHandleInterruptsMethod(context)
            ]);

    [Pure]
    private static MemberDeclarationSyntax CreateInterruptModesField(GeneratorContext context)
    {
        var initializer = EqualsValueClause(
            CollectionExpression(
                SeparatedList<CollectionElementSyntax>(
                    context.Interrupts.Modes
                        .Select(m => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(m.Steps[0].Index)))).ToArray())));

        var variableDeclarator = VariableDeclarator(Identifier(InterruptModeStepTableFieldName)).WithInitializer(initializer);

        var variable = VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax)
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable).AddModifiers(Private, Static, ReadOnly);
    }

    [Pure]
    private static MethodDeclarationSyntax CreateHandleInterruptsMethod(GeneratorContext context)
    {
        var statements = StatementGenerator.GenerateStatements(context, context.Interrupts.Handle).ToList();
        statements.Add(ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression)));

        return MethodDeclaration(
                Bool,
                Identifier(HandleInterruptsMethodName))
            .WithParameterList(ParameterList(
            [
                CreateEmulatorParameter(context),
                Parameter(Identifier(ActionRequiredParameterName)).WithType(IdentifierName(ActionRequiredEnumName)).WithModifiers([Ref])
            ]))
            .AddModifiers(Private, Static)
            .WithBody(Block(statements))
            .WithLeadingTrivia(NewlineComment);
    }
}