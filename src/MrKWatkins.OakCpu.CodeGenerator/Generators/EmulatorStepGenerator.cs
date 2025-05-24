using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepGenerator : EmulatorClassGenerator
{
    private const string StepMethodName = "Step";
    private const string ActionVariableName = "action";
    public static readonly EmulatorStepGenerator Instance = new();

    private EmulatorStepGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(CreateStepMethod(input));

    [Pure]
    private static MethodDeclarationSyntax CreateStepMethod(GeneratorInput input) =>
        SyntaxFactory
            .MethodDeclaration(
                SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                SyntaxFactory.Identifier(StepMethodName))
            .AddModifiers(Public)
            .WithBody(SyntaxFactory.Block(
                CreateActionVariable(),
                CreateSwitch(input),
                SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(ActionVariableName))));

    [Pure]
    private static SwitchStatementSyntax CreateSwitch(GeneratorInput input)
    {
        var sections = new[]
        {
            CreateOpcodeReadRequest(input, 0),
            CreateHandleOpcodeReadRequest(1),
        };

        return SyntaxFactory
            .SwitchStatement(CreatePostIncrementExpression(StepIndexFieldName))
            .AddSections(sections);
    }

    [Pure]
    private static SwitchSectionSyntax CreateHandleOpcodeReadRequest(int index) => CreateSwitchSection(index, true, CreateSetMember(LastOpcodeFieldName, DataPropertyName));

    [Pure]
    private static SwitchSectionSyntax CreateOpcodeReadRequest(GeneratorInput input, int index) => CreateMemoryReadRequest(index, input.ProgramCounter.FieldName);

    [Pure]
    private static SwitchSectionSyntax CreateMemoryReadRequest(int index, string addressField) => CreateMemoryReadRequest(index, SyntaxFactory.IdentifierName(addressField));

    [Pure]
    private static SwitchSectionSyntax CreateMemoryReadRequest(int index, ExpressionSyntax addressExpression) =>
        CreateSwitchSection(index, true, CreateSetMember(AddressPropertyName, addressExpression), CreateSetAction("MemoryRead"));

    [Pure]
    private static SwitchSectionSyntax CreateSwitchSection(int index, params StatementSyntax[] statements) => CreateSwitchSection(index, true, statements);

    [Pure]
    private static SwitchSectionSyntax CreateSwitchSection(int index, bool needsBreak, params StatementSyntax[] statements)
    {
        var section = SyntaxFactory.SwitchSection()
            .AddLabels(SyntaxFactory.CaseSwitchLabel(GetNumericLiteralExpression(index)))
            .AddStatements(statements);

        if (needsBreak)
        {
            section = section.AddStatements(SyntaxFactory.BreakStatement());
        }

        return section;
    }

    [Pure]
    private static StatementSyntax CreateActionVariable() =>
        SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(ActionVariableName))
                        .WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                                    SyntaxFactory.IdentifierName(ActionRequiredNone)))))));


    [Pure]
    private static StatementSyntax CreateSetAction(string name) =>
        SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(ActionVariableName),
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(ActionRequiredEnumName),
                    SyntaxFactory.IdentifierName(name))));

    [Pure]
    private static ExpressionSyntax CreatePostIncrementExpression(string field) =>
        SyntaxFactory.PostfixUnaryExpression(
            SyntaxKind.PostIncrementExpression,
            SyntaxFactory.IdentifierName(field));
}