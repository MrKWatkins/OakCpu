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
        // $"The opcode {prefix != 0 ? $"0x{opcode:X2} " : ""}0x{Data:X2} is not supported."

        // $"0x{opcode:X2} "
        var ternaryFormat = GenerateInterpolatedString(
            InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, "0x", "0x", TriviaList())),
            GenerateX2Interpolation(DataMember.Prefix),
            InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, " ", " ", TriviaList())));

        // (prefix != 0 ? $"0x{opcode:X2} " : "")
        var ternary = ParenthesizedExpression(
            ConditionalExpression(
                BinaryExpression(SyntaxKind.NotEqualsExpression, IdentifierName(DataMember.Prefix.Name), LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0))),
                ternaryFormat,
                LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(""))));

        var interpolatedString = GenerateInterpolatedString(
            InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, "The opcode ", "The opcode ", TriviaList())),
            Interpolation(ternary),
            InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, "0x", "0x", TriviaList())),
            GenerateX2Interpolation(DataMember.Data),
            InterpolatedStringText(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, " is not supported.", " is not supported.", TriviaList())));

        return ThrowStatement(ObjectCreationExpression(IdentifierName("System.NotSupportedException"))
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(interpolatedString)))));
    }

    [Pure]
    private static InterpolatedStringExpressionSyntax GenerateInterpolatedString(params InterpolatedStringContentSyntax[] contents) =>
        InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
            .WithContents(List(contents))
            .WithStringEndToken(Token(SyntaxKind.InterpolatedStringEndToken));

    [Pure]
    private static InterpolationSyntax GenerateX2Interpolation(DataMember dataMember)
    {
        var colonToken = Token(SyntaxKind.ColonToken);
        var formatToken = Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, "X2", "X2", TriviaList());
        var formatClause = InterpolationFormatClause(colonToken, formatToken);

        return Interpolation(IdentifierName(dataMember.Name)).WithFormatClause(formatClause);
    }
}