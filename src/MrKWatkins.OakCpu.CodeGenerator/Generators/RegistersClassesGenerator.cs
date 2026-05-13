using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class RegistersClassesGenerator : TypeGenerator
{
    public static readonly RegistersClassesGenerator Instance = new();

    private RegistersClassesGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.Registers(context);

    [Pure]
    public override IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) => GenerateOneFilePerType(context);

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        foreach (var category in GetAllCategories(context))
        {
            yield return CreateBaseClass(context, category);
            yield return CreateConcreteClass(context, category, Class.Name.StepRegisters(context, category), Class.Identifier.Emulator(context), Class.Name.StepRegisters);
            yield return CreateConcreteClass(context, category, Class.Name.InstructionRegisters(context, category), Class.Identifier.InstructionEmulator(context), Class.Name.InstructionRegisters);
        }
    }

    [Pure]
    private static ClassDeclarationSyntax CreateBaseClass(GeneratorContext context, string? category)
    {
        var members = new List<MemberDeclarationSyntax>();
        if (category == null)
        {
            var categories = GetCategories(context).ToArray();
            if (categories.Length != 0)
            {
                members.Add(CreateBaseConstructor(context, categories));
            }

            members.AddRange(CreateCategoryProperties(context, categories));
        }

        members.AddRange(CreateRegisterProperties(context, category, createOverrideProperty: false));

        var summary = category == null
            ? $"Provides access to the {context.Cpu.Name} registers."
            : $"Provides access to the {context.Cpu.Name} {category.ToLowerInvariant()} registers.";

        return WithXmlDocumentation(
            ClassDeclaration(Class.Name.Registers(context, category))
                .AddModifiers(Public, Abstract)
                .AddMembers(members.ToArray()),
            summary);
    }

    [Pure]
    private static ClassDeclarationSyntax CreateConcreteClass(
        GeneratorContext context,
        string? category,
        string className,
        TypeSyntax emulatorType,
        Func<GeneratorContext, string?, string> getConcreteClassName)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(emulatorType),
            CreateConcreteConstructor(context, category, className, emulatorType, getConcreteClassName)
        };
        members.AddRange(CreateRegisterProperties(context, category, createOverrideProperty: true));

        return ClassDeclaration(className)
            .AddModifiers(Internal, Sealed)
            .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName(Class.Name.Registers(context, category))))))
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateBaseConstructor(GeneratorContext context, IReadOnlyList<string> categories) =>
        WithXmlDocumentation(
            ConstructorDeclaration(Class.Name.Registers(context))
                .WithModifiers(TokenList(Protected))
                .WithParameterList(
                    ParameterList(
                        SeparatedList(
                            categories.Select(
                                category => Parameter(Identifier(ToCamelCase(category)))
                                    .WithType(IdentifierName(Class.Name.Registers(context, category)))))))
                .WithBody(
                    Block(
                        categories.Select(
                            category => ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName(category),
                                    IdentifierName(ToCamelCase(category))))))),
            $"Initializes a new {Class.Name.Registers(context)} instance.",
            parameters: categories.ToDictionary(ToCamelCase, category => $"The {context.Cpu.Name} {category.ToLowerInvariant()} registers."));

    [Pure]
    private static ConstructorDeclarationSyntax CreateConcreteConstructor(
        GeneratorContext context,
        string? category,
        string className,
        TypeSyntax emulatorType,
        Func<GeneratorContext, string?, string> getConcreteClassName)
    {
        var constructor = ConstructorDeclaration(className)
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(emulatorType))))
            .WithBody(Block(CreateAssignEmulatorFieldExpression()));

        if (category != null)
        {
            return constructor;
        }

        var categories = GetCategories(context).ToArray();
        if (categories.Length == 0)
        {
            return constructor;
        }

        return constructor.WithInitializer(
            ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                ArgumentList(
                    SeparatedList(
                        categories.Select(
                            categoryName => Argument(
                                ObjectCreationExpression(IdentifierName(getConcreteClassName(context, categoryName)))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(CreateEmulatorArgument())))))))));
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateCategoryProperties(GeneratorContext context, IReadOnlyList<string> categories) =>
        categories.Select(category => WithXmlDocumentation(
            CreateGetOnlyProperty(context, Class.Name.Registers(context, category), category),
            $"Gets the {context.Cpu.Name} {category.ToLowerInvariant()} registers."));

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateRegisterProperties(GeneratorContext context, string? category, bool createOverrideProperty) =>
        context.Configuration.Registers.Values
            .Where(register => register.HasRegisterClassProperty && register.Category == category)
            .OrderBy(register => register.Name)
            .Select(register => CreateRegisterProperty(context, register, createOverrideProperty));

    [Pure]
    private static PropertyDeclarationSyntax CreateRegisterProperty(GeneratorContext context, Register register, bool createOverrideProperty)
    {
        if (!createOverrideProperty)
        {
            return WithXmlDocumentation(CreateAbstractGetSetProperty(register.Type.TypeSyntax(), register.PropertyName), register.Documentation);
        }

        var memberAccessExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(register.FieldName));

        return CreateOverrideGetSetProperty(
            context,
            register.Type.TypeSyntax(),
            register.PropertyName,
            memberAccessExpression,
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                memberAccessExpression,
                IdentifierName("value")));
    }

    [Pure]
    private static IEnumerable<string?> GetAllCategories(GeneratorContext context) => new string?[] { null }.Concat(GetCategories(context));

    [Pure]
    private static IEnumerable<string> GetCategories(GeneratorContext context) => context.Configuration.Registers.Values
        .Where(register => register.Category != null)
        .Select(register => register.Category!)
        .Distinct()
        .OrderBy(category => category);

    [Pure]
    private static string ToCamelCase(string value) => char.ToLowerInvariant(value[0]) + value[1..];
}