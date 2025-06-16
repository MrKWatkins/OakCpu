using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStaticDataMembersAndConstructorGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorStaticDataMembersAndConstructorGenerator Instance = new();

    private EmulatorStaticDataMembersAndConstructorGenerator()
    {
    }

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var members = context.Configuration.OpcodeStepTables
            .Select(CreateOpcodeStepTableField)
            .Append(CreateStaticConstructor(context));

        return classDeclaration.AddMembers(members.ToArray());
    }

    [Pure]
    private static MemberDeclarationSyntax CreateOpcodeStepTableField(OpcodeStepTable opcodeStepTable)
    {
        var variableDeclarator = VariableDeclarator(Identifier(opcodeStepTable.FieldName));

        var variable = VariableDeclaration(PreDefinedDataMember.OpcodeStepTable.TypeSyntax)
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable).AddModifiers(Private, Static, ReadOnly);
    }

    [MustUseReturnValue]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax> { CreateLittleEndianStatement(context) };

        // This is not totally generic. But it does support everything I need. Doesn't support:
        // - Duplicates with no prefix, i.e. different opcodes.
        // - Duplicates between an item in an opcode table and out.
        var duplicatesWithPrefix = context.Instructions
            .Where(i => i.OpcodeTable == null)
            .SelectMany(i => i.Duplicates)
            .GroupBy(d => context.Configuration.OpcodeStepTables.GetForPrefix(d.Prefix!.Value)) // Assumes no duplicates without a prefix.
            .Select(g => (g.Key, Items: g.Select(d => (d.Opcode, d.Step))));

        var duplicatesWithinOpcodeTable = context.Instructions
            .Where(i => i.OpcodeTable != null)
            .GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction)
            .Select(g => (g.Key, Items: g.SelectMany(i => i.Duplicates.Select(d => (d.Opcode, d.Step)))));

        var duplicatesByOpcodeTable = duplicatesWithPrefix.Concat(duplicatesWithinOpcodeTable).ToDictionary(x => x.Key, x => x.Items.ToList());

        foreach (var group in context.Instructions.GroupBy(context.Configuration.OpcodeStepTables.GetForInstruction))
        {
            statements.Add(CreateOpcodeStepTableInitializationStatement(group.Key, group, duplicatesByOpcodeTable.TryGetValue(group.Key, out var duplicates) ? duplicates : []));
        }

        // Static constructor
        return ConstructorDeclaration(GetEmulatorClassName(context))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
    }

    [Pure]
    private static StatementSyntax CreateLittleEndianStatement(GeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(BitConverter).Namespace);
        context.RequiredUsings.Add(typeof(NotSupportedException).Namespace);

        // throw new NotSupportedException("Only little endian systems are supported.");
        var throwStatement = ThrowStatement(
                ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("Only little endian systems are supported.")))))));

        // #pragma warning disable CA1065
        var pragmaDisable = PragmaWarningDirectiveTrivia(
                Token(SyntaxKind.DisableKeyword),
                SeparatedList<ExpressionSyntax>()
                    .Add(IdentifierName("CA1065")),
                true);

        // #pragma warning enable CA1065
        var pragmaRestore = PragmaWarningDirectiveTrivia(
                Token(SyntaxKind.RestoreKeyword),
                SeparatedList<ExpressionSyntax>()
                    .Add(IdentifierName("CA1065")),
                true);

        // !BitConverter.IsLittleEndian
        var condition = PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(BitConverter)),
                    IdentifierName(nameof(BitConverter.IsLittleEndian))));

        // if (!BitConverter.IsLittleEndian)
        // {
        // #pragma warning disable CA1065
        //  throw new NotSupportedException("Only little endian systems are supported.");
        // #pragma warning restore CA1065
        // }
        return IfStatement(
                condition,
                Block(
                    List<StatementSyntax>()
                        .Add(throwStatement
                            .WithLeadingTrivia(Trivia(pragmaDisable))
                            .WithTrailingTrivia(Trivia(pragmaRestore)))));
    }

    [Pure]
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(OpcodeStepTable opcodeStepTable, [InstantHandle] IEnumerable<Instruction> instructions, IReadOnlyList<(byte Opcode, Step Step)> duplicates)
    {
        var stepIndices = Enumerable.Repeat(65535, 256).ToArray();
        foreach (var instruction in instructions)
        {
            stepIndices[instruction.Opcode] = instruction.Steps.First().Index;
        }

        foreach (var duplicate in duplicates)
        {
            stepIndices[duplicate.Opcode] = duplicate.Step.Index;
        }

        var literals = stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))));

        var value = CollectionExpression(SeparatedList<CollectionElementSyntax>(literals));

        var assignment = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(opcodeStepTable.FieldName),
            value);

        return ExpressionStatement(assignment);
    }
}