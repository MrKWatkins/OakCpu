using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorResetGenerator : EmulatorClassGenerator
{
    private const string ResetMethodName = "Reset";

    public static readonly EmulatorResetGenerator Instance = new();

    private EmulatorResetGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(GenerateReset(context));

    [Pure]
    private static MemberDeclarationSyntax GenerateReset(GeneratorContext context)
    {
        var statements = GenerateResetOpcodeStepTable(context)
            .Concat(GenerateResetDataMembers(context))
            .Concat(GenerateResetRegisters(context));

        return MethodDeclaration(Void, Identifier(ResetMethodName))
            .WithModifiers([Public])
            .WithBody(Block(statements));
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
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(m => m.Name)
            .Select(m => GenerateReset(m.FieldName, m.Type));

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
    private static LiteralExpressionSyntax ResetValue(DataType type) => type switch
    {
        DataType.U8 => GenerateNumericLiteralExpression(0),
        DataType.U16 => GenerateNumericLiteralExpression(0),
        DataType.Bool => LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword)),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}