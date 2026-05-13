using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Creates reset statements shared by the step and instruction emulator reset generators.
/// </summary>
internal static class ResetSyntax
{
    /// <summary>
    /// Resets the opcode step table back to the no-prefix table.
    /// </summary>
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateResetOpcodeStepTable(GeneratorContext context)
    {
        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName)));
    }

    /// <summary>
    /// Resets all top-level register fields to their default values.
    /// </summary>
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateResetRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(r => r.Parent == null)
            .OrderBy(r => r.FieldOffset)
            .Select(r => GenerateReset(r.FieldName, DataType.U8));

    /// <summary>
    /// Resets a single field to the default literal for its data type.
    /// </summary>
    [Pure]
    public static StatementSyntax GenerateReset(string fieldName, DataType type) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(fieldName),
                type.DefaultLiteral()));
}