using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class TableGeneration
{
    [Pure]
    internal static IEnumerable<MemberDeclarationSyntax> CreateStepTableFields(GeneratorContext context, Func<SequenceGroup, string> getSequenceGroupStepTableFieldName) =>
        context.Configuration.OpcodeStepTables
            .Select(CreateOpcodeStepTableField)
            .Concat(context.SequenceGroups.Values.OrderBy(group => group.Name).Select(group => CreateSequenceGroupStepTableField(getSequenceGroupStepTableFieldName(group))));

    [MustUseReturnValue]
    internal static IfStatementSyntax CreateLittleEndianStatement(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(BitConverter));
        context.RequiredUsings.Add(typeof(NotSupportedException));

        var throwStatement = ThrowStatement(
            ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal("Only little endian systems are supported.")))))));

        var pragmaDisable = PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), SeparatedList<ExpressionSyntax>().Add(IdentifierName("CA1065")), true);
        var pragmaRestore = PragmaWarningDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), SeparatedList<ExpressionSyntax>().Add(IdentifierName("CA1065")), true);

        return IfStatement(
            PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(BitConverter)),
                    IdentifierName(nameof(BitConverter.IsLittleEndian)))),
            Block(
                List<StatementSyntax>()
                    .Add(throwStatement.WithLeadingTrivia(Trivia(pragmaDisable)).WithTrailingTrivia(Trivia(pragmaRestore)))));
    }

    [MustUseReturnValue]
    internal static IEnumerable<StatementSyntax> CreateTableInitializationStatements(
        FileGeneratorContext context,
        Func<FileGeneratorContext, OpcodeStepTable, IEnumerable<Instruction>, IReadOnlyList<(byte Opcode, Step Step)>, StatementSyntax> createOpcodeStepTableInitializationStatement,
        Func<FileGeneratorContext, SequenceGroup, StatementSyntax> createSequenceGroupStepTableInitializationStatement)
    {
        var duplicatesByOpcodeTable = CreateDuplicatesByOpcodeTable(context);

        foreach (var group in context.GeneratorContext.Instructions.GroupBy(context.GeneratorContext.Configuration.OpcodeStepTables.GetForInstruction))
        {
            yield return createOpcodeStepTableInitializationStatement(context, group.Key, group, duplicatesByOpcodeTable.TryGetValue(group.Key, out var duplicates) ? duplicates : []);
        }

        foreach (var sequenceGroup in context.GeneratorContext.SequenceGroups.Values.OrderBy(group => group.Name))
        {
            yield return createSequenceGroupStepTableInitializationStatement(context, sequenceGroup);
        }
    }

    [Pure]
    private static MemberDeclarationSyntax CreateOpcodeStepTableField(OpcodeStepTable opcodeStepTable) =>
        FieldDeclaration(VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax).WithVariables([VariableDeclarator(Identifier(opcodeStepTable.FieldName))]))
            .AddModifiers(Private, Static, ReadOnly);

    [Pure]
    private static MemberDeclarationSyntax CreateSequenceGroupStepTableField(string fieldName) =>
        FieldDeclaration(VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax).WithVariables([VariableDeclarator(Identifier(fieldName))]))
            .AddModifiers(Private, Static, ReadOnly);

    [Pure]
    private static IReadOnlyDictionary<OpcodeStepTable, IReadOnlyList<(byte Opcode, Step Step)>> CreateDuplicatesByOpcodeTable(GeneratorContext context)
    {
        var duplicatesWithPrefix = context.Instructions
            .Where(i => i.OpcodeTable == null)
            .SelectMany(i => i.Duplicates)
            .GroupBy(d => context.Configuration.OpcodeStepTables.GetForPrefix(d.Prefix!.Value))
            .Select(g => (g.Key, Items: (IReadOnlyList<(byte Opcode, Step Step)>)g.Select(d => (d.Opcode, d.Step)).ToList()));

        var duplicatesWithinOpcodeTable = context.Instructions
            .Where(i => i.OpcodeTable != null)
            .GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction)
            .Select(g => (g.Key, Items: (IReadOnlyList<(byte Opcode, Step Step)>)g.SelectMany(i => i.Duplicates.Select(d => (d.Opcode, d.Step))).ToList()));

        return duplicatesWithPrefix.Concat(duplicatesWithinOpcodeTable).ToDictionary(x => x.Key, x => x.Items);
    }
}