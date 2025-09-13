using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class Generator
{
    protected const string ActionRequiredEnumName = "ActionRequired";
    protected const string StepStructName = "Step";
    protected const string StepHandlerFieldName = "Handler";
    protected const string StepNextStepFieldName = "NextStep";
    protected const string StepActionRequiredFieldName = "ActionRequired";
    protected const string EmulatorParameterName = SyntaxHelpers.EmulatorParameterName;
    protected const string ActionRequiredParameterName = SyntaxHelpers.ActionRequiredParameterName;
    protected const string ErrorFunctionName = "Error";
    protected const string HandleInterruptsMethodName = "HandleInterrupts";
    protected const string InterruptModeStepTableFieldName = "InterruptModeStepTable";
    private const string StepFunctionPrefix = "Step_";

    // Filthy hackery to put some newlines and indents where we want because NormalizeWhitespace will remove any normal whitespace we add.
    protected static readonly string NewlineCommentText = SyntaxHelpers.NewlineCommentText;
    protected static readonly SyntaxTrivia NewlineComment = SyntaxHelpers.NewlineComment;
    protected static readonly SyntaxTrivia IndentComment = SyntaxHelpers.IndentComment;

    private protected Generator()
    {
    }

    [Pure]
    protected static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) =>
        SyntaxHelpers.InitializeVariableStatement(variable, value);

    [Pure]
    protected static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value, TypeSyntax type) =>
        SyntaxHelpers.InitializeVariableStatement(variable, value, type);

    [Pure]
    protected static ExpressionSyntax CreateArrayGetWithoutBoundsCheck(GeneratorContext context, ExpressionSyntax array, ExpressionSyntax index) =>
        SyntaxHelpers.CreateArrayGetWithoutBoundsCheck(context.RequiredUsings, array, index);

    [Pure]
    protected static PredefinedTypeSyntax Bool => CommonSyntax.Bool;

    [Pure]
    protected static PredefinedTypeSyntax Byte => CommonSyntax.Byte;

    [Pure]
    protected static PredefinedTypeSyntax Int => CommonSyntax.Int;

    [Pure]
    protected static PredefinedTypeSyntax UShort => CommonSyntax.UShort;

    [Pure]
    protected static TypeSyntax Void => CommonSyntax.Void;

    [Pure]
    protected static SyntaxToken Field => CommonSyntax.Field;

    [Pure]
    protected static SyntaxToken Internal => CommonSyntax.Internal;

    [Pure]
    protected static SyntaxToken Partial => CommonSyntax.Partial;

    [Pure]
    protected static SyntaxToken Private => CommonSyntax.Private;

    [Pure]
    protected static SyntaxToken Public => CommonSyntax.Public;

    [Pure]
    protected static SyntaxToken ReadOnly => CommonSyntax.ReadOnly;

    [Pure]
    protected static SyntaxToken Ref => CommonSyntax.Ref;

    [Pure]
    protected static SyntaxToken Sealed => CommonSyntax.Sealed;

    [Pure]
    protected static SyntaxToken Semicolon => CommonSyntax.Semicolon;

    [Pure]
    protected static SyntaxToken Static => CommonSyntax.Static;

    [Pure]
    protected static SyntaxToken Unsafe => CommonSyntax.Unsafe;

    [Pure]
    protected static SyntaxToken GenerateBinaryLiteral(byte value) => SyntaxHelpers.GenerateBinaryLiteral(value);

    [Pure]
    protected static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) => SyntaxHelpers.GenerateBinaryLiteralExpression(value);

    [Pure]
    protected static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) => SyntaxHelpers.GenerateNumericLiteralExpression(value);

    [Pure]
    protected static IdentifierNameSyntax EmulatorMemberIdentifier(string name) => SyntaxHelpers.EmulatorMemberIdentifier(name);

    [Pure]
    protected static ArgumentSyntax CreateEmulatorArgument() => SyntaxHelpers.CreateEmulatorArgument();

    [Pure]
    protected static string GetStepFunctionName(Step step) => $"{StepFunctionPrefix}{step.Index}";
}