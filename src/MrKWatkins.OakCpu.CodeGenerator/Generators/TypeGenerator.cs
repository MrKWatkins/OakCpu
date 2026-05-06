using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratedNames;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class TypeGenerator
{
    protected const string EmulatorFieldName = "emulator";

    private protected TypeGenerator()
    {
    }

    public static readonly IReadOnlyList<TypeGenerator> AllGenerators = [
        ActionRequiredGenerator.Instance,
        EmulatorInstanceDataMembersAndConstructorGenerator.Instance,
        EmulatorInterruptsGenerator.Instance,
        InstructionEmulatorStateGenerator.Instance,
        InstructionEmulatorInterruptsGenerator.Instance,
        InstructionEmulatorGenerator.Instance,
        InstructionEmulatorResetGenerator.Instance,
        InstructionEmulatorSerializationGenerator.Instance,
        InstructionEmulatorTablesGenerator.Instance,
        EmulatorOverlapsGenerator.Instance,
        EmulatorResetGenerator.Instance,
        EmulatorSerializationGenerator.Instance,
        EmulatorStepsInitializationGenerator.Instance,
        EmulatorStepsGenerator.Instance,
        FlagsClassGenerator.Instance,
        InterruptsClassGenerator.Instance,
        StepStructGenerator.Instance,
        RegistersClassesGenerator.Instance];

    [Pure]
    public string GetFileName(GeneratorContext context) => $"{GetBaseFileName(context)}.generated.cs";

    [Pure]
    protected abstract string GetBaseFileName(GeneratorContext context);

    [Pure]
    public virtual IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) => [new(GetFileName(context), Generate(context))];

    [Pure]
    public string Generate(GeneratorContext context)
    {
        var compilationUnit = GenerateCompilationUnit(context);
        return Generate(compilationUnit);
    }

    [Pure]
    protected static string Generate(CompilationUnitSyntax compilationUnit) => GeneratedCodeFormatter.Format(compilationUnit);

    [Pure]
    public CompilationUnitSyntax GenerateCompilationUnit(GeneratorContext context)
    {
        var fileContext = context.WithRequiredUsings();
        return GenerateCompilationUnit(fileContext, CreateTypes(fileContext));
    }

    [MustUseReturnValue]
    protected virtual IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        yield return CreateType(context);
    }

    [MustUseReturnValue]
    protected virtual BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        throw new NotImplementedException($"{nameof(CreateType)} is not implemented and {nameof(CreateTypes)} has not been overridden.");
    }

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(GeneratorContext context, string typeName, string propertyName) =>
        CreateGetOnlyProperty(context, IdentifierName(typeName), propertyName, TokenList(Public));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetOnlyProperty(GeneratorContext context, TypeSyntax type, string propertyName, SyntaxTokenList modifiers) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(modifiers)
            .WithAccessorList(
                AccessorList(
                [
                    CreateGetAccessor(context.RequiredUsings)
                ]));

    [Pure]
    protected static PropertyDeclarationSyntax CreateAbstractGetOnlyProperty(TypeSyntax type, string propertyName) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(TokenList(Public, Abstract))
            .WithAccessorList(
                AccessorList(
                [
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Semicolon)
                ]));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(GeneratorContext context, TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression) =>
        CreateGetSetProperty(context, type, propertyName, getExpression, setExpression, TokenList(Public));

    [Pure]
    protected static PropertyDeclarationSyntax CreateGetSetProperty(GeneratorContext context, TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression, SyntaxTokenList modifiers) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(modifiers)
            .WithAccessorList(
                AccessorList(List(
                [
                    CreateGetAccessor(context.RequiredUsings, getExpression),
                    CreateSetAccessor(context.RequiredUsings, setExpression)
                ])));

    [Pure]
    protected static PropertyDeclarationSyntax CreateAbstractGetSetProperty(TypeSyntax type, string propertyName) =>
        PropertyDeclaration(type, Identifier(propertyName))
            .WithModifiers(TokenList(Public, Abstract))
            .WithAccessorList(
                AccessorList(
                [
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Semicolon),

                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Semicolon)
                ]));

    [Pure]
    protected static PropertyDeclarationSyntax CreateOverrideGetSetProperty(GeneratorContext context, TypeSyntax type, string propertyName, ExpressionSyntax getExpression, ExpressionSyntax setExpression) =>
        CreateGetSetProperty(context, type, propertyName, getExpression, setExpression, TokenList(Public, Override));

    [Pure]
    protected static T WithXmlDocumentation<T>(T node, Definitions.Documentation documentation)
        where T : SyntaxNode =>
        documentation.IsEmpty ? node : WithXmlDocumentation(node, documentation.Summary);

    [Pure]
    protected static T WithXmlDocumentation<T>(T node, string summary, string? remarks = null, IReadOnlyDictionary<string, string>? parameters = null, string? returns = null)
        where T : SyntaxNode
    {
        var lines = new List<string>();
        AddXmlElement(lines, "summary", summary);

        if (!string.IsNullOrWhiteSpace(remarks))
        {
            AddXmlElement(lines, "remarks", remarks);
        }

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                AddXmlElement(lines, $"param name=\"{parameter.Key}\"", parameter.Value);
            }
        }

        if (!string.IsNullOrWhiteSpace(returns))
        {
            AddXmlElement(lines, "returns", returns);
        }

        var trivia = CreateXmlDocumentationTrivia(lines)
            .AddRange(node.GetLeadingTrivia());
        if (node is MemberDeclarationSyntax { AttributeLists.Count: > 0 } member)
        {
            var firstToken = member.GetFirstToken();
            var documentedToken = firstToken.WithLeadingTrivia(trivia.AddRange(firstToken.LeadingTrivia));
            return (T)(SyntaxNode)member.ReplaceToken(firstToken, documentedToken);
        }

        return node.WithLeadingTrivia(trivia);
    }

    [Pure]
    private static SyntaxTriviaList CreateXmlDocumentationTrivia(IEnumerable<string> lines)
    {
        var trivia = new List<SyntaxTrivia>();
        foreach (var line in lines)
        {
            trivia.Add(Comment(line));
            trivia.Add(EndOfLine(Environment.NewLine));
        }

        return TriviaList(trivia);
    }

    private static void AddXmlElement(List<string> lines, string tag, string value)
    {
        lines.Add($"/// <{tag}>");
        foreach (var line in value.Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            lines.Add($"/// {EscapeXml(line)}");
        }
        lines.Add($"/// </{tag.Split(' ')[0]}>");
    }

    [Pure]
    private static string EscapeXml(string value) =>
        value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

    [Pure]
    protected static FieldDeclarationSyntax CreateEmulatorField(TypeSyntax emulatorType) =>
        FieldDeclaration(
                VariableDeclaration(emulatorType)
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(EmulatorFieldName)))))
            .WithModifiers(TokenList(Private, ReadOnly));
    [Pure]
    protected static ParameterSyntax CreateInstructionActionCallbackParameter() =>
        Parameter(Identifier(InstructionActionCallbackParameterName))
            .WithType(
                GenericName(Identifier("Action"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                            [
                                IdentifierName(ActionRequiredEnumName),
                                Token(SyntaxKind.CommaToken),
                                UShortType,
                                Token(SyntaxKind.CommaToken),
                                ByteType
                            ]))));

    [Pure]
    protected static FunctionPointerTypeSyntax CreateInstructionHandlerType(GeneratorContext context) =>
        FunctionPointerType(
            null,
            FunctionPointerParameterList(
            [
                FunctionPointerParameter(IdentifierName(GetInstructionEmulatorClassName(context))),
                FunctionPointerParameter(
                    GenericName(Identifier("Action"))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SeparatedList<TypeSyntax>(
                                [
                                    IdentifierName(ActionRequiredEnumName),
                                    Token(SyntaxKind.CommaToken),
                                    UShortType,
                                    Token(SyntaxKind.CommaToken),
                                    ByteType
                                ])))),
                FunctionPointerParameter(IntType)
            ]));

    [Pure]
    protected static FunctionPointerTypeSyntax CreateOverlapHandlerType(GeneratorContext context) =>
        FunctionPointerType(
            null,
            FunctionPointerParameterList(
            [
                FunctionPointerParameter(IdentifierName(GetEmulatorClassName(context))),
                FunctionPointerParameter(VoidType)
            ]));

    [Pure]
    protected IReadOnlyList<GeneratedFile> GenerateOneFilePerType(GeneratorContext context)
    {
        var typeCount = CreateTypes(context.WithRequiredUsings()).Count();

        return Enumerable
            .Range(0, typeCount)
            .Select(index =>
            {
                var fileContext = context.WithRequiredUsings();
                var type = CreateTypes(fileContext).ElementAt(index);
                return new GeneratedFile($"{type.Identifier.ValueText}.generated.cs", Generate(GenerateCompilationUnit(fileContext, [type])));
            })
            .ToList();
    }

    [Pure]
    private static CompilationUnitSyntax GenerateCompilationUnit(GeneratorContext context, IEnumerable<BaseTypeDeclarationSyntax> types)
    {
        var members = new List<MemberDeclarationSyntax> { context.CreateRootNamespaceDeclaration() };
        members.AddRange(types);

        return CompilationUnit()
            .AddUsings(context.RequiredUsings.CreateUsingDirectives().ToArray())
            .AddMembers(members.ToArray())
            .NormalizeWhitespace();
    }

    public sealed record GeneratedFile(string FileName, string Source);
}