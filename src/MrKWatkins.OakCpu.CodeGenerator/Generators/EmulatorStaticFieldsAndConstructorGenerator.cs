using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStaticFieldsAndConstructorGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorStaticFieldsAndConstructorGenerator Instance = new();

    private EmulatorStaticFieldsAndConstructorGenerator()
    {
    }

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration)
    {
        var members = input.OpcodeStepTables
            .Select(CreateOpcodeStepTableField)
            .Append(CreateStaticConstructor(requiredUsings, input));

        return classDeclaration.AddMembers(members.ToArray());
    }

    [Pure]
    private static MemberDeclarationSyntax CreateOpcodeStepTableField(OpcodeStepTable opcodeStepTable)
    {
        var variableDeclarator = VariableDeclarator(Identifier(opcodeStepTable.FieldName));

        var variable = VariableDeclaration(DataMember.OpcodeStepTable.MemberTypeSyntax)
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable).AddModifiers(Private, Static, ReadOnly);
    }

    [MustUseReturnValue]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(HashSet<string> requiredUsings, GeneratorInput input)
    {
        var statements = new List<StatementSyntax> { CreateLittleEndianStatement(requiredUsings) };

        foreach (var group in input.Instructions.GroupBy(input.OpcodeStepTables.GetForInstruction))
        {
            statements.Add(CreateOpcodeStepTableInitializationStatement(group.Key, group));
        }

        // Static constructor
        return ConstructorDeclaration(GetEmulatorClassName(input))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(statements));
    }

    [Pure]
    private static StatementSyntax CreateLittleEndianStatement(HashSet<string> requiredUsings)
    {

        requiredUsings.Add(typeof(BitConverter).Namespace);
        requiredUsings.Add(typeof(NotSupportedException).Namespace);

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
    private static StatementSyntax CreateOpcodeStepTableInitializationStatement(OpcodeStepTable opcodeStepTable, [InstantHandle] IEnumerable<Instruction> instructions)
    {
        var stepIndices = Enumerable.Repeat(65535, 256).ToArray();
        foreach (var instruction in instructions)
        {
            // TODO: If instruction.opcode in prefixes do something.
            stepIndices[instruction.Opcode] = instruction.Steps.First().Index;
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