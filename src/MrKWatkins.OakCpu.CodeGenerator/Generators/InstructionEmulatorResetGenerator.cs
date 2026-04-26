using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorResetGenerator : TypeGenerator
{
    private const string ResetMethodName = "Reset";
    public static readonly InstructionEmulatorResetGenerator Instance = new();

    private InstructionEmulatorResetGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetInstructionEmulatorClassName(context)}.reset";

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context) =>
        ClassDeclaration(GetInstructionEmulatorClassName(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(GenerateReset(context));

    [Pure]
    private static MemberDeclarationSyntax GenerateReset(GeneratorContext context)
    {
        var statements = GenerateResetOpcodeStepTable(context)
            .Concat(GenerateResetDataMembers(context))
            .Append(GenerateResetPendingInterruptStep())
            .Concat(GenerateResetRegisters(context));

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(ResetMethodName))
                .WithModifiers([Public])
                .WithBody(Block(statements)),
            $"Resets the {context.Cpu.Name} CPU state.");
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateResetOpcodeStepTable(GeneratorContext context)
    {
        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName)));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateResetDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable && m != PreDefinedDataMember.CurrentStep)
            .OrderBy(m => m.Name)
            .Select(m => GenerateReset(m.FieldName, m.Type));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateResetRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(r => r.Parent == null)
            .OrderBy(r => r.FieldOffset)
            .Select(r => GenerateReset(r.FieldName, DataType.U8));

    [Pure]
    private static StatementSyntax GenerateResetPendingInterruptStep() =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("pendingInterruptStep"),
                GenerateNumericLiteralExpression(0)));

    [Pure]
    private static StatementSyntax GenerateReset(string fieldName, DataType type) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(fieldName),
                type switch
                {
                    DataType.U8 => GenerateNumericLiteralExpression(0),
                    DataType.U16 => GenerateNumericLiteralExpression(0),
                    DataType.Bool => LiteralExpression(SyntaxKind.FalseLiteralExpression),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                }));

}