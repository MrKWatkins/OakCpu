using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorTablesGenerator : TypeGenerator
{
    public static readonly InstructionEmulatorTablesGenerator Instance = new();

    private InstructionEmulatorTablesGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetInstructionEmulatorClassName(context)}.tables";

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = context.Configuration.OpcodeStepTables
            .Select(CreateOpcodeStepTableField)
            .Concat(context.SequenceGroups.Values.OrderBy(group => group.Name).Select(CreateSequenceGroupStepTableField))
            .Prepend(CreateInstructionsField(context))
            .Append(CreateStaticConstructor(context))
            .ToArray();

        return ClassDeclaration(GetInstructionEmulatorClassName(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(members);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateInstructionsField(GeneratorContext context) =>
        FieldDeclaration(
                VariableDeclaration(
                        ArrayType(CreateInstructionHandlerType(context))
                            .WithRankSpecifiers([ArrayRankSpecifier([OmittedArraySizeExpression()])]))
                    .WithVariables([VariableDeclarator(Identifier(InstructionHandlersFieldName))]))
            .AddModifiers(Private, Static, ReadOnly);

    [Pure]
    private static MemberDeclarationSyntax CreateOpcodeStepTableField(OpcodeStepTable opcodeStepTable) =>
        FieldDeclaration(VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax).WithVariables([VariableDeclarator(Identifier(opcodeStepTable.FieldName))]))
            .AddModifiers(Private, Static, ReadOnly);

    [Pure]
    private static MemberDeclarationSyntax CreateSequenceGroupStepTableField(SequenceGroup sequenceGroup) =>
        FieldDeclaration(VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax).WithVariables([VariableDeclarator(Identifier(GetSequenceGroupStepTableFieldName(sequenceGroup)))]))
            .AddModifiers(Private, Static, ReadOnly);

    [Pure]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            CreateLittleEndianStatement(context),
            CreateInstructionsInitializationStatement(context)
        };

        var duplicatesWithPrefix = context.Instructions
            .Where(i => i.OpcodeTable == null)
            .SelectMany(i => i.Duplicates)
            .GroupBy(d => context.Configuration.OpcodeStepTables.GetForPrefix(d.Prefix!.Value))
            .Select(g => (g.Key, Items: g.Select(d => (d.Opcode, d.Step))));

        var duplicatesWithinOpcodeTable = context.Instructions
            .Where(i => i.OpcodeTable != null)
            .GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction)
            .Select(g => (g.Key, Items: g.SelectMany(i => i.Duplicates.Select(d => (d.Opcode, d.Step)))));

        var duplicatesByOpcodeTable = duplicatesWithPrefix.Concat(duplicatesWithinOpcodeTable).ToDictionary(x => x.Key, x => x.Items.ToList());

        foreach (var group in context.Instructions.GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction))
        {
            statements.Add(CreateOpcodeStepTableInitializationStatement(context, group.Key, group, duplicatesByOpcodeTable.TryGetValue(group.Key, out var duplicates) ? duplicates : []));
        }

        foreach (var sequenceGroup in context.SequenceGroups.Values.OrderBy(group => group.Name))
        {
            statements.Add(CreateSequenceGroupStepTableInitializationStatement(context, sequenceGroup));
        }

        return ConstructorDeclaration(GetInstructionEmulatorClassName(context))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
    }

    [Pure]
    private static StatementSyntax CreateInstructionsInitializationStatement(GeneratorContext context)
    {
        var dispatchableSequences = InstructionEmulatorGenerator.GetDispatchableSequences(context).ToList();
        var handlers = Enumerable.Repeat<ExpressionSyntax>(PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(ErrorMethodName)), context.InstructionEmulatorDispatchCount).ToArray();

        foreach (var sequence in dispatchableSequences)
        {
            handlers[context.GetInstructionEmulatorSequenceIndex(sequence)] =
                PrefixUnaryExpression(SyntaxKind.AddressOfExpression, IdentifierName(InstructionEmulatorGenerator.GetInstructionMethodName(context, sequence)));
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(InstructionHandlersFieldName),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(handlers.Select(ExpressionElement)))));
    }

    [Pure]
    private static IfStatementSyntax CreateLittleEndianStatement(GeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(BitConverter).Namespace!);
        context.RequiredUsings.Add(typeof(NotSupportedException).Namespace!);

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

    [Pure]
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(GeneratorContext context, OpcodeStepTable opcodeStepTable, IEnumerable<Instruction> instructions, IReadOnlyList<(byte Opcode, Step Step)> duplicates)
    {
        var stepIndices = Enumerable.Repeat(context.InstructionEmulatorErrorIndex, 256).ToArray();

        if (opcodeStepTable == OpcodeStepTable.NoPrefix)
        {
            foreach (var prefixJump in context.PrefixJumps.Values)
            {
                stepIndices[prefixJump.Prefix] = context.GetInstructionEmulatorPrefixIndex(prefixJump.Prefix);
            }
        }
        else if (opcodeStepTable.Prefix is { } prefix && context.PrefixJumps.ContainsKey(prefix))
        {
            stepIndices[prefix] = context.GetInstructionEmulatorPrefixIndex(prefix);
        }

        foreach (var instruction in instructions)
        {
            stepIndices[instruction.Opcode] = context.GetInstructionEmulatorSequenceIndex(instruction);
        }

        foreach (var duplicate in duplicates)
        {
            stepIndices[duplicate.Opcode] = context.GetInstructionEmulatorSequenceIndex(duplicate.Step.Sequence);
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(opcodeStepTable.FieldName),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))))))));
    }

    [Pure]
    private static StatementSyntax CreateSequenceGroupStepTableInitializationStatement(GeneratorContext context, SequenceGroup sequenceGroup)
    {
        var stepIndices = Enumerable.Repeat(context.InstructionEmulatorErrorIndex, sequenceGroup.MaximumNumber + 1).ToArray();
        foreach (var member in sequenceGroup.Members)
        {
            stepIndices[member.Key] = context.GetInstructionEmulatorSequenceIndex(member.Value);
        }

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(GetSequenceGroupStepTableFieldName(sequenceGroup)),
                CollectionExpression(SeparatedList<CollectionElementSyntax>(stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))))))));
    }
}
