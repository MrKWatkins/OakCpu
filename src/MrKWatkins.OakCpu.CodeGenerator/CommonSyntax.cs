using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

internal static class CommonSyntax
{
    [Pure]
    public static SyntaxToken Abstract => Token(SyntaxKind.AbstractKeyword);

    [Pure]
    public static PredefinedTypeSyntax BoolType => PredefinedType(Token(SyntaxKind.BoolKeyword));

    [Pure]
    public static PredefinedTypeSyntax ByteType => PredefinedType(Token(SyntaxKind.ByteKeyword));

    [Pure]
    public static PredefinedTypeSyntax IntType => PredefinedType(Token(SyntaxKind.IntKeyword));

    [Pure]
    public static PredefinedTypeSyntax UShortType => PredefinedType(Token(SyntaxKind.UShortKeyword));

    [Pure]
    public static TypeSyntax VoidType => PredefinedType(Token(SyntaxKind.VoidKeyword));

    [Pure]
    public static SyntaxToken Field => Token(SyntaxKind.FieldKeyword);

    [Pure]
    public static SyntaxToken Internal => Token(SyntaxKind.InternalKeyword);

    [Pure]
    public static SyntaxToken Override => Token(SyntaxKind.OverrideKeyword);

    [Pure]
    public static SyntaxToken Partial => Token(SyntaxKind.PartialKeyword);

    [Pure]
    public static SyntaxToken Private => Token(SyntaxKind.PrivateKeyword);

    [Pure]
    public static SyntaxToken Protected => Token(SyntaxKind.ProtectedKeyword);

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
    public static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) => InitializeVariableStatement(variable, value, IdentifierName("var"));

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
    public static AttributeSyntax CreateMethodImplAttribute(ISet<string> requiredUsings, MethodImplOptions options) => CreateMethodImplAttribute(requiredUsings, options.ToString());

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
    public static IdentifierNameSyntax EmulatorMemberIdentifier(string name) => IdentifierName($"emulator.{name}");

    [Pure]
    public static ArgumentSyntax CreateEmulatorArgument() => Argument(IdentifierName("emulator"));

    [Pure]
    public static SyntaxToken GenerateBinaryLiteral(byte value) => Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    public static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, GenerateBinaryLiteral(value));

    [Pure]
    public static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    [Pure]
    public static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
        // this.emulator = emulator;
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName("emulator")),
                IdentifierName("emulator")));

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