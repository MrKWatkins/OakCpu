using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorStepGenerator : EmulatorClassGenerator
{
    private const string StepMethodName = "Step";
    public static readonly EmulatorStepGenerator Instance = new();

    private EmulatorStepGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(CreateStepMethod(input));

    [Pure]
    private static MethodDeclarationSyntax CreateStepMethod(GeneratorInput input) =>
        MethodDeclaration(
                IdentifierName(ActionRequiredEnumName),
                Identifier(StepMethodName))
            .AddModifiers(Public)
            .WithBody(Block(
                CreateSwitch(input),
                CreateThrowNotSupportedException()));

    [Pure]
    private static SwitchStatementSyntax CreateSwitch(GeneratorInput input)
    {
        var sections = input.AllSteps.Select(step => CreateSwitchSection(input, step)).ToArray();

        return SwitchStatement(CreatePostIncrementExpression(DataMember.Step.Name))
            .AddSections(sections);
    }

    [Pure]
    private static SwitchSectionSyntax CreateSwitchSection(GeneratorInput input, Step step)
    {
        var statements = StatementGenerator.GenerateStatementSyntaxes(input, step).ToArray();

        return SwitchSection()
            .AddLabels(CaseSwitchLabel(GenerateNumericLiteralExpression(step.Index)))
            .AddStatements(Block(statements))
            .WithLeadingTrivia(Comment($"// {step.Name}"));
    }

    [Pure]
    private static ExpressionSyntax CreatePostIncrementExpression(string field) =>
        PostfixUnaryExpression(
            SyntaxKind.PostIncrementExpression,
            IdentifierName(field));

    [Pure]
    private static StatementSyntax CreateThrowNotSupportedException()
    {
        // $"The opcode 0x{opcode:X2} is not supported."
        var start = InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, "The opcode 0x", "The opcode 0x", TriviaList()));


        var colonToken = Token(SyntaxKind.ColonToken);
        var formatToken = Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, "X2", "X2", TriviaList());
        var formatClause = InterpolationFormatClause(colonToken, formatToken);

        var interpolation = Interpolation(IdentifierName(DataMember.Opcode.Name)).WithFormatClause(formatClause);

        var end = InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, " is not supported.", " is not supported.", TriviaList()));

        var interpolatedString = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
            .WithContents(List<InterpolatedStringContentSyntax>([start, interpolation, end]))
            .WithStringEndToken(Token(SyntaxKind.InterpolatedStringEndToken));

        return ThrowStatement(ObjectCreationExpression(IdentifierName("System.NotSupportedException"))
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(interpolatedString)))));
    }
}