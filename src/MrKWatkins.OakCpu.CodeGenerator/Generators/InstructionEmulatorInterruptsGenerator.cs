using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratedNames;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorInterruptsGenerator : TypeGenerator
{
    public static readonly InstructionEmulatorInterruptsGenerator Instance = new();

    private InstructionEmulatorInterruptsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetInstructionEmulatorClassName(context)}.interrupts";

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        StatementSyntax[] statements =
        [
            .. StatementGenerator.GenerateStatements(context, context.Interrupts.Handle, instructionEmulatorMode: true),
            ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression))
        ];

        return ClassDeclaration(GetInstructionEmulatorClassName(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(
                MethodDeclaration(BoolType, Identifier(HandleInterruptsMethodName))
                    .WithModifiers([Private, Static])
                    .WithParameterList(
                        ParameterList(
                        [CreateInstructionEmulatorParameter(context)]))
                    .WithBody(Block(statements)));
    }
}