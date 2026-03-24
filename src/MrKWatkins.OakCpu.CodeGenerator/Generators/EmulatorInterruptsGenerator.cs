using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorInterruptsGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorInterruptsGenerator Instance = new();

    private EmulatorInterruptsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetEmulatorClassName(context)}.interrupts";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .WithMembers(
            [
                CreateHandleInterruptsMethod(context)
            ]);

    [Pure]
    private static MethodDeclarationSyntax CreateHandleInterruptsMethod(GeneratorContext context)
    {
        var statements = StatementGenerator.GenerateStatements(context, context.Interrupts.Handle).ToList();
        statements.Add(ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression)));

        return MethodDeclaration(
                BoolType,
                Identifier(HandleInterruptsMethodName))
            .WithParameterList(ParameterList(
            [
                CreateEmulatorParameter(context),
                Parameter(Identifier(ActionRequiredParameterName)).WithType(IdentifierName(ActionRequiredEnumName)).WithModifiers([Ref])
            ]))
            .AddModifiers(Private, Static)
            .WithBody(Block(statements));
    }
}
