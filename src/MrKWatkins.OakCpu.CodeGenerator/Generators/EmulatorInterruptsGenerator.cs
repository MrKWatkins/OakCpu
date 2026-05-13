using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorInterruptsGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorInterruptsGenerator Instance = new();

    private EmulatorInterruptsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.Emulator(context)}.interrupts";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration
            .WithMembers(
            [
                CreateHandleInterruptsMethod(context)
            ]);

    [Pure]
    private static MethodDeclarationSyntax CreateHandleInterruptsMethod(GeneratorContext context)
    {
        StatementSyntax[] statements =
        [
            .. StatementGenerator.GenerateStatements(context, context.Interrupts.Handle),
            ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression))
        ];

        return MethodDeclaration(
                BoolType,
                Identifier(Method.Name.HandleInterrupts))
            .WithParameterList(ParameterList(
            [
                Parameter.Syntax.Emulator(context),
                Parameter(Identifier(Parameter.Name.ActionRequired)).WithType(IdentifierName(TypeName.ActionRequiredEnum)).WithModifiers([Ref])
            ]))
            .AddModifiers(Private, Static)
            .WithBody(Block(statements));
    }
}