using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorInterruptsGenerator : TypeGenerator
{
    public static readonly InstructionEmulatorInterruptsGenerator Instance = new();

    private InstructionEmulatorInterruptsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.InstructionEmulator(context)}.interrupts";

    protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        StatementSyntax[] statements =
        [
            .. StatementGenerator.GenerateStatements(context, context.GeneratorContext.Interrupts.Handle, instructionEmulatorMode: true),
            ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression))
        ];

        return ClassDeclaration(Class.Name.InstructionEmulator(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(
                MethodDeclaration(BoolType, Identifier(Method.Name.HandleInterrupts))
                    .WithModifiers([Private, Static])
                    .WithParameterList(
                        ParameterList(
                        [Parameter.Syntax.InstructionEmulator(context)]))
                    .WithBody(Block(statements)));
    }
}