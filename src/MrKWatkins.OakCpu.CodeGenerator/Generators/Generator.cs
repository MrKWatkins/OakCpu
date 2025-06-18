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
    protected const string EmulatorParameterName = "emulator";
    private const string StepFunctionPrefix = "Step_";

    // Filthy hackery to put some newlines and indents where we want because NormalizeWhitespace will remove any normal whitespace we add.
    protected static readonly SyntaxTrivia NewlineComment = Comment("// Newline");
    protected static readonly SyntaxTrivia IndentComment = Comment("// Indent");

    private protected Generator()
    {
    }

    [Pure]
    protected static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) =>
        LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
            .WithVariables(SingletonSeparatedList(
                VariableDeclarator(Identifier(variable))
                    .WithInitializer(EqualsValueClause(value)))));

    [Pure]
    protected static ExpressionSyntax CreateArrayGetWithoutBoundsCheck(GeneratorContext context, ExpressionSyntax array, ExpressionSyntax index)
    {
        context.RequiredUsings.Add("System.Runtime.CompilerServices");
        context.RequiredUsings.Add("System.Runtime.InteropServices");

        // Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(values), index);
        return InvocationExpression(
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Unsafe"), IdentifierName("Add")))
            .WithArgumentList(
                ArgumentList([
                    Argument(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("MemoryMarshal"), IdentifierName("GetArrayDataReference")))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(array)))))
                        .WithRefKindKeyword(Token(SyntaxKind.RefKeyword)),
                    Argument(index)]));
    }

    [Pure]
    protected static PredefinedTypeSyntax Bool => PredefinedType(Token(SyntaxKind.BoolKeyword));

    [Pure]
    protected static PredefinedTypeSyntax Byte => PredefinedType(Token(SyntaxKind.ByteKeyword));

    [Pure]
    protected static PredefinedTypeSyntax UShort => PredefinedType(Token(SyntaxKind.UShortKeyword));

    [Pure]
    protected static TypeSyntax Void => PredefinedType(Token(SyntaxKind.VoidKeyword));

    [Pure]
    protected static SyntaxToken Field => Token(SyntaxKind.FieldKeyword);

    [Pure]
    protected static SyntaxToken Internal => Token(SyntaxKind.InternalKeyword);

    [Pure]
    protected static SyntaxToken Partial => Token(SyntaxKind.PartialKeyword);

    [Pure]
    protected static SyntaxToken Private => Token(SyntaxKind.PrivateKeyword);

    [Pure]
    protected static SyntaxToken Public => Token(SyntaxKind.PublicKeyword);

    [Pure]
    protected static SyntaxToken ReadOnly => Token(SyntaxKind.ReadOnlyKeyword);

    [Pure]
    protected static SyntaxToken Sealed => Token(SyntaxKind.SealedKeyword);

    [Pure]
    protected static SyntaxToken Semicolon => Token(SyntaxKind.SemicolonToken);

    [Pure]
    protected static SyntaxToken Static => Token(SyntaxKind.StaticKeyword);

    [Pure]
    protected static SyntaxToken Unsafe => Token(SyntaxKind.UnsafeKeyword);

    [Pure]
    protected static SyntaxToken GenerateBinaryLiteral(byte value) => Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    protected static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, GenerateBinaryLiteral(value));

    [Pure]
    protected static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    [Pure]
    protected static IdentifierNameSyntax EmulatorMemberIdentifier(string name) => IdentifierName($"{EmulatorParameterName}.{name}");

    [Pure]
    protected static ArgumentSyntax CreateEmulatorArgument() => Argument(IdentifierName(EmulatorParameterName));

    [Pure]
    protected static string GetStepFunctionName(Step step) => $"{StepFunctionPrefix}{step.Index}";
}