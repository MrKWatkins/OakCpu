using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

/// <summary>
/// Provides common helper methods for generating Roslyn syntax nodes.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class SyntaxHelpers
{
    // Constants for common parameter and field names.
    public const string EmulatorParameterName = "emulator";
    public const string ActionRequiredParameterName = "actionRequired";
    public const string EmulatorFieldName = "emulator";

    // Common syntax trivia for formatting.
    public static readonly string NewlineCommentText = "// Newline";
    public static readonly SyntaxTrivia NewlineComment = Comment(NewlineCommentText);
    public static readonly SyntaxTrivia IndentComment = Comment("// Indent");

    /// <summary>
    /// Creates a local variable declaration statement with initialization.
    /// </summary>
    [Pure]
    public static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value) =>
        InitializeVariableStatement(variable, value, IdentifierName("var"));

    /// <summary>
    /// Creates a local variable declaration statement with explicit type and initialization.
    /// </summary>
    [Pure]
    public static StatementSyntax InitializeVariableStatement(string variable, ExpressionSyntax value, TypeSyntax type) =>
        LocalDeclarationStatement(VariableDeclaration(type)
            .WithVariables(SingletonSeparatedList(
                VariableDeclarator(Identifier(variable))
                    .WithInitializer(EqualsValueClause(value)))));

    /// <summary>
    /// Creates an unsafe array access expression using Unsafe.Add and MemoryMarshal.GetArrayDataReference.
    /// </summary>
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

    /// <summary>
    /// Creates a MethodImpl attribute with the specified options.
    /// </summary>
    [Pure]
    public static AttributeSyntax CreateMethodImplAttribute(ISet<string> requiredUsings, MethodImplOptions options) =>
        CreateMethodImplAttribute(requiredUsings, options.ToString());

    /// <summary>
    /// Creates a MethodImpl attribute with the specified options string.
    /// </summary>
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

    /// <summary>
    /// Creates an identifier expression for accessing a member of the emulator parameter.
    /// </summary>
    [Pure]
    public static IdentifierNameSyntax EmulatorMemberIdentifier(string name) =>
        IdentifierName($"{EmulatorParameterName}.{name}");

    /// <summary>
    /// Creates an argument for passing the emulator parameter.
    /// </summary>
    [Pure]
    public static ArgumentSyntax CreateEmulatorArgument() =>
        Argument(IdentifierName(EmulatorParameterName));

    /// <summary>
    /// Creates a binary literal token for the specified byte value.
    /// </summary>
    [Pure]
    public static SyntaxToken GenerateBinaryLiteral(byte value) =>
        Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    /// <summary>
    /// Creates a binary literal expression for the specified byte value.
    /// </summary>
    [Pure]
    public static LiteralExpressionSyntax GenerateBinaryLiteralExpression(byte value) =>
        LiteralExpression(SyntaxKind.NumericLiteralExpression, GenerateBinaryLiteral(value));

    /// <summary>
    /// Creates a numeric literal expression for the specified integer value.
    /// </summary>
    [Pure]
    public static LiteralExpressionSyntax GenerateNumericLiteralExpression(int value) =>
        LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    /// <summary>
    /// Creates an assignment statement for assigning emulator field.
    /// </summary>
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

    /// <summary>
    /// Creates an expression statement for creating a new object and assigning it to a property.
    /// </summary>
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