using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

internal static class CommonSyntax
{
    [Pure]
    internal static SyntaxToken Abstract => Token(SyntaxKind.AbstractKeyword);

    [Pure]
    internal static PredefinedTypeSyntax BoolType => PredefinedType(Token(SyntaxKind.BoolKeyword));

    [Pure]
    internal static PredefinedTypeSyntax ByteType => PredefinedType(Token(SyntaxKind.ByteKeyword));

    [Pure]
    internal static PredefinedTypeSyntax IntType => PredefinedType(Token(SyntaxKind.IntKeyword));

    [Pure]
    internal static PredefinedTypeSyntax UShortType => PredefinedType(Token(SyntaxKind.UShortKeyword));

    [Pure]
    internal static TypeSyntax VoidType => PredefinedType(Token(SyntaxKind.VoidKeyword));

    [Pure]
    internal static SyntaxToken Field => Token(SyntaxKind.FieldKeyword);

    [Pure]
    internal static SyntaxToken Internal => Token(SyntaxKind.InternalKeyword);

    [Pure]
    internal static SyntaxToken Override => Token(SyntaxKind.OverrideKeyword);

    [Pure]
    internal static SyntaxToken Partial => Token(SyntaxKind.PartialKeyword);

    [Pure]
    internal static SyntaxToken Private => Token(SyntaxKind.PrivateKeyword);

    [Pure]
    internal static SyntaxToken Protected => Token(SyntaxKind.ProtectedKeyword);

    [Pure]
    internal static SyntaxToken Public => Token(SyntaxKind.PublicKeyword);

    [Pure]
    internal static SyntaxToken ReadOnly => Token(SyntaxKind.ReadOnlyKeyword);

    [Pure]
    internal static SyntaxToken Ref => Token(SyntaxKind.RefKeyword);

    [Pure]
    internal static SyntaxToken Sealed => Token(SyntaxKind.SealedKeyword);

    [Pure]
    internal static SyntaxToken Semicolon => Token(SyntaxKind.SemicolonToken);

    [Pure]
    internal static SyntaxToken Static => Token(SyntaxKind.StaticKeyword);

    [Pure]
    internal static SyntaxToken Unsafe => Token(SyntaxKind.UnsafeKeyword);

    [Pure]
    internal static SyntaxToken Virtual => Token(SyntaxKind.VirtualKeyword);

    [Pure]
    internal static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) => InitializeVariableStatement(variable, value, IdentifierName("var"));

    [Pure]
    internal static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value, TypeSyntax type) =>
        LocalDeclarationStatement(VariableDeclaration(type)
            .WithVariables(SingletonSeparatedList(
                VariableDeclarator(Identifier(variable))
                    .WithInitializer(EqualsValueClause(value)))));

    [MustUseReturnValue]
    internal static ExpressionSyntax CreateArrayGetWithoutBoundsCheck(RequiredUsings requiredUsings, ExpressionSyntax array, ExpressionSyntax index)
    {
        requiredUsings.Add(typeof(Unsafe));
        requiredUsings.Add(typeof(MemoryMarshal));

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

    [MustUseReturnValue]
    internal static AttributeSyntax CreateMethodImplAttribute(RequiredUsings requiredUsings, MethodImplOptions options) => CreateMethodImplAttribute(requiredUsings, options.ToString());

    [MustUseReturnValue]
    internal static AttributeSyntax CreateMethodImplAttribute(RequiredUsings requiredUsings, string options)
    {
        requiredUsings.Add(typeof(MethodImplAttribute));

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
    internal static ExpressionSyntax EmulatorMemberIdentifier(string name) =>
        MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("emulator"),
            IdentifierName(name));

    [Pure]
    internal static ArgumentSyntax CreateEmulatorArgument() => Argument(IdentifierName("emulator"));

    [Pure]
    private static SyntaxToken GenerateBinaryLiteral(byte value) => Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    internal static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, GenerateBinaryLiteral(value));

    [Pure]
    internal static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    [Pure]
    internal static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
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
    internal static ExpressionStatementSyntax CreateNewObjectAndAssignToProperty(string propertyName, string classToCreateName, params ExpressionSyntax[] constructorArguments) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(propertyName),
                ObjectCreationExpression(IdentifierName(classToCreateName))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(constructorArguments.Select(Argument).ToArray())))));

    [MustUseReturnValue]
    internal static AttributeListSyntax CreateAggressiveInliningAttributeList(RequiredUsings requiredUsings) =>
        AttributeList([CreateMethodImplAttribute(requiredUsings, MethodImplOptions.AggressiveInlining)]);

    [MustUseReturnValue]
    internal static AccessorDeclarationSyntax CreateGetAccessor(RequiredUsings requiredUsings) =>
        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithAttributeLists([CreateAggressiveInliningAttributeList(requiredUsings)])
            .WithSemicolonToken(Semicolon);

    [MustUseReturnValue]
    internal static AccessorDeclarationSyntax CreateGetAccessor(RequiredUsings requiredUsings, ExpressionSyntax getExpression) =>
        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithExpressionBody(ArrowExpressionClause(getExpression))
            .WithAttributeLists([CreateAggressiveInliningAttributeList(requiredUsings)])
            .WithSemicolonToken(Semicolon);

    [MustUseReturnValue]
    internal static AccessorDeclarationSyntax CreateSetAccessor(RequiredUsings requiredUsings, ExpressionSyntax setExpression, SyntaxTokenList? modifiers = null)
    {
        var accessor = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithExpressionBody(ArrowExpressionClause(setExpression))
            .WithAttributeLists([CreateAggressiveInliningAttributeList(requiredUsings)])
            .WithSemicolonToken(Semicolon);

        if (modifiers is { Count: > 0 })
        {
            accessor = accessor.WithModifiers(modifiers.Value);
        }

        return accessor;
    }
}