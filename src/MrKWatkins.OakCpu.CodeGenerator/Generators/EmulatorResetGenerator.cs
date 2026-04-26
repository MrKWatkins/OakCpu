using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorResetGenerator : EmulatorClassGenerator
{
    private const string ResetMethodName = "Reset";

    public static readonly EmulatorResetGenerator Instance = new();

    private EmulatorResetGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetEmulatorClassName(context)}.reset";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(GenerateReset(context));

    [Pure]
    private static MemberDeclarationSyntax GenerateReset(GeneratorContext context)
    {
        var statements = GenerateResetOpcodeStepTable(context)
            .Concat(GenerateResetDataMembers(context))
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
            AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName)));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateResetDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Concat<DataMember>([PreDefinedDataMember.OverlapPipeline])
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(m => m.Name)
            .Select(m => m == PreDefinedDataMember.OverlapPipeline
                ? GenerateResetOverlapPipeline(context)
                : GenerateReset(m.FieldName, m.Type));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateResetRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(r => r.Parent == null)
            .OrderBy(r => r.FieldOffset)
            .Select(r => GenerateReset(r.FieldName, DataType.U8));

    [Pure]
    private static StatementSyntax GenerateReset(string fieldName, DataType type) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(fieldName), ResetValue(type)));

    [Pure]
    private static StatementSyntax GenerateResetOverlapPipeline(GeneratorContext context) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName),
                DefaultExpression(CreateOverlapHandlerType(context))));

    [Pure]
    private static LiteralExpressionSyntax ResetValue(DataType type) => type switch
    {
        DataType.U8 => GenerateNumericLiteralExpression(0),
        DataType.U16 => GenerateNumericLiteralExpression(0),
        DataType.Bool => LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword)),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}