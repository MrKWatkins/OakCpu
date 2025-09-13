using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

internal static class CommonSyntax
{
    // Constants for common parameter and field names.
    public const string EmulatorParameterName = "emulator";
    public const string ActionRequiredParameterName = "actionRequired";
    public const string EmulatorFieldName = "emulator";

    // Common syntax trivia for formatting.
    public static readonly string NewlineCommentText = "// Newline";
    public static readonly SyntaxTrivia NewlineComment = Comment(NewlineCommentText);
    public static readonly SyntaxTrivia IndentComment = Comment("// Indent");

    [Pure]
    public static PredefinedTypeSyntax Bool => PredefinedType(Token(SyntaxKind.BoolKeyword));

    [Pure]
    public static PredefinedTypeSyntax Byte => PredefinedType(Token(SyntaxKind.ByteKeyword));

    [Pure]
    public static PredefinedTypeSyntax Int => PredefinedType(Token(SyntaxKind.IntKeyword));

    [Pure]
    public static PredefinedTypeSyntax UShort => PredefinedType(Token(SyntaxKind.UShortKeyword));

    [Pure]
    public static TypeSyntax Void => PredefinedType(Token(SyntaxKind.VoidKeyword));

    [Pure]
    public static SyntaxToken Field => Token(SyntaxKind.FieldKeyword);

    [Pure]
    public static SyntaxToken Internal => Token(SyntaxKind.InternalKeyword);

    [Pure]
    public static SyntaxToken Partial => Token(SyntaxKind.PartialKeyword);

    [Pure]
    public static SyntaxToken Private => Token(SyntaxKind.PrivateKeyword);

    [Pure]
    public static SyntaxToken Public => Token(SyntaxKind.PublicKeyword);

    [Pure]
    public static SyntaxToken ReadOnly => Token(SyntaxKind.ReadOnlyKeyword);

    [Pure]
    public static SyntaxToken Ref => Token(SyntaxKind.RefKeyword);

    [Pure]
    public static SyntaxToken Sealed => Token(SyntaxKind.SealedKeyword);

    [Pure]
    public static SyntaxToken Semicolon => Token(SyntaxKind.SemicolonToken);

    [Pure]
    public static SyntaxToken Static => Token(SyntaxKind.StaticKeyword);

    [Pure]
    public static SyntaxToken Unsafe => Token(SyntaxKind.UnsafeKeyword);

    [Pure]
    public static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) =>
        InitializeVariableStatement(variable, value, IdentifierName("var"));

    [Pure]
    public static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value, TypeSyntax type) =>
        LocalDeclarationStatement(VariableDeclaration(type)
            .WithVariables(SingletonSeparatedList(
                VariableDeclarator(Identifier(variable))
                    .WithInitializer(EqualsValueClause(value)))));

    [Pure]
    public static ExpressionSyntax CreateArrayGetWithoutBoundsCheck(ISet<string> requiredUsings, ExpressionSyntax array, ExpressionSyntax index)
    {
        requiredUsings.Add("System.Runtime.CompilerServices");
        requiredUsings.Add("System.Runtime.InteropServices");

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
    public static AttributeSyntax CreateMethodImplAttribute(ISet<string> requiredUsings, MethodImplOptions options) =>
        CreateMethodImplAttribute(requiredUsings, options.ToString());

    [Pure]
    public static AttributeSyntax CreateMethodImplAttribute(ISet<string> requiredUsings, string options)
    {
        requiredUsings.Add("System.Runtime.CompilerServices");

        return Attribute(
            IdentifierName("MethodImpl"),
            AttributeArgumentList(
                SingletonSeparatedList(
                    AttributeArgument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(nameof(MethodImplOptions)),
                            IdentifierName(options))))));
    }

    [Pure]
    public static IdentifierNameSyntax EmulatorMemberIdentifier(string name) =>
        IdentifierName($"{EmulatorParameterName}.{name}");

    [Pure]
    public static ArgumentSyntax CreateEmulatorArgument() =>
        Argument(IdentifierName(EmulatorParameterName));

    [Pure]
    public static SyntaxToken GenerateBinaryLiteral(byte value) =>
        Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    public static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) =>
        LiteralExpression(SyntaxKind.NumericLiteralExpression, GenerateBinaryLiteral(value));

    [Pure]
    public static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) =>
        LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    [Pure]
    public static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
        // this.emulator = emulator;
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(EmulatorFieldName)),
                IdentifierName(EmulatorFieldName)));

    [Pure]
    public static ExpressionStatementSyntax CreateNewObjectAndAssignToProperty(string propertyName, string classToCreateName, params ExpressionSyntax[] constructorArguments) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(propertyName),
                ObjectCreationExpression(IdentifierName(classToCreateName))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(constructorArguments.Select(Argument).ToArray())))));
}