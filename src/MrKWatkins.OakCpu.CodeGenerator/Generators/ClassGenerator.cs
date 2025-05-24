using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class ClassGenerator
{
    protected const string EmulatorClassName = "Z80Emulator";
    protected const string FlagsClassName = "Z80Flags";
    protected const string RegistersClassName = "Z80Registers";
    protected const string EmulatorFieldName = "emulator";

    protected static class ActionRequired
    {
        public const string EnumName = "ActionRequired";
        public const string None = "None";
        public const string MemoryRead = "MemoryRead";
        public const string MemoryWrite = "MemoryWrite";
        public const string IORead = "IORead";
        public const string IOWrite = "IOWrite";
        public static readonly IReadOnlyList<string> Members = [None, MemoryRead, MemoryWrite, IORead, IOWrite];
    }

    private protected ClassGenerator()
    {
    }

    public static readonly IReadOnlyList<ClassGenerator> AllGenerators = [
        ActionRequiredGenerator.Instance,
        EmulatorFieldsPropertiesAndConstructorGenerator.Instance,
        EmulatorStepGenerator.Instance,
        FlagsClassGenerator.Instance,
        RegistersClassesGenerator.Instance];

    public string FileName => GetType().Name.Substring(0, GetType().Name.Length - "Generator".Length);

    [Pure]
    public CompilationUnitSyntax Generate(GeneratorInput input)
    {
        var requiredUsings = new HashSet<string>();

        var classDeclarations = CreateTypes(requiredUsings, input).ToArray<MemberDeclarationSyntax>();

        return SyntaxFactory
            .CompilationUnit()
            .AddUsings(requiredUsings.OrderBy(n => n).Select(CreateUsingStatement).ToArray())
            .AddMembers(
                input
                    .CreateRootNamespaceDeclaration()
                    .AddMembers(classDeclarations))
            .NormalizeWhitespace();
    }

    [MustUseReturnValue]
    protected virtual IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(HashSet<string> requiredUsings, GeneratorInput input)
    {
        yield return CreateType(requiredUsings, input);
    }

    [MustUseReturnValue]
    protected virtual BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input)
    {
        throw new NotImplementedException($"{nameof(CreateType)} is not implemented and {nameof(CreateTypes)} has not been overridden.");
    }

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(string typeName, string propertyName) =>
        SyntaxFactory
            .PropertyDeclaration(SyntaxFactory.IdentifierName(typeName), SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon))));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string propertyName) =>
        SyntaxFactory
            .PropertyDeclaration(type, SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List([
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Semicolon),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Semicolon)
                ])));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression) =>
        SyntaxFactory
            .PropertyDeclaration(type, SyntaxFactory.Identifier(propertyName))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List(
                [
                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(getExpression))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),

                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(setExpression))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                ])));
    [Pure]
    protected static FieldDeclarationSyntax CreateEmulatorField() =>
        SyntaxFactory
            .FieldDeclaration(
                SyntaxFactory
                    .VariableDeclaration(SyntaxFactory.IdentifierName(EmulatorClassName))
                    .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(EmulatorFieldName)))))
            .WithModifiers(SyntaxFactory.TokenList(Private, ReadOnly));

    [Pure]
    protected static ExpressionStatementSyntax CreateNewObjectAndAssignToProperty(string propertyName, string classToCreateName, params ExpressionSyntax[] constructorArguments) =>
        SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(propertyName),
                SyntaxFactory
                    .ObjectCreationExpression(SyntaxFactory.IdentifierName(classToCreateName))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(constructorArguments.Select(SyntaxFactory.Argument).ToArray())))));

    [Pure]
    protected static ExpressionStatementSyntax CreateAssignEmulatorFieldExpression() =>
        // this.emulator = emulator;
        SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.ThisExpression(),
                    SyntaxFactory.IdentifierName(EmulatorFieldName)),
                SyntaxFactory.IdentifierName(EmulatorFieldName)));

    [Pure]
    protected static PredefinedTypeSyntax Bool => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

    [Pure]
    protected static PredefinedTypeSyntax Byte => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword));

    [Pure]
    protected static PredefinedTypeSyntax UShort => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword));

    [Pure]
    protected static SyntaxToken Field => SyntaxFactory.Token(SyntaxKind.FieldKeyword);

    [Pure]
    protected static SyntaxToken Internal => SyntaxFactory.Token(SyntaxKind.InternalKeyword);

    [Pure]
    protected static SyntaxToken Partial => SyntaxFactory.Token(SyntaxKind.PartialKeyword);

    [Pure]
    protected static SyntaxToken Private => SyntaxFactory.Token(SyntaxKind.PrivateKeyword);

    [Pure]
    protected static SyntaxToken Public => SyntaxFactory.Token(SyntaxKind.PublicKeyword);

    [Pure]
    protected static SyntaxToken ReadOnly => SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword);

    [Pure]
    protected static SyntaxToken Sealed => SyntaxFactory.Token(SyntaxKind.SealedKeyword);

    [Pure]
    protected static SyntaxToken Semicolon => SyntaxFactory.Token(SyntaxKind.SemicolonToken);

    [Pure]
    protected static SyntaxToken GetBinaryLiteral(byte value) => SyntaxFactory.Literal($"0b{Convert.ToString(value, 2).PadLeft(8, '0')}", value);

    [Pure]
    protected static LiteralExpressionSyntax GetBinaryLiteralExpression(byte value) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, GetBinaryLiteral(value));

    [Pure]
    protected static LiteralExpressionSyntax GetNumericLiteralExpression(int value) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));

    [Pure]
    protected static string GetRegistersClassName(string? category = null) => $"Z80{category}Registers";

    [Pure]
    private static UsingDirectiveSyntax CreateUsingStatement(string ns) => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(ns));
}