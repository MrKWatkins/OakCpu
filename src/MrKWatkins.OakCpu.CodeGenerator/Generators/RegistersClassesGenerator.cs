using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class RegistersClassesGenerator : TypeGenerator
{
    public static readonly RegistersClassesGenerator Instance = new();

    private RegistersClassesGenerator()
    {
    }

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        yield return CreateClass(context, null);

        foreach (var category in GetCategories(context))
        {
            yield return CreateClass(context, category);
        }
    }

    private static ClassDeclarationSyntax CreateClass(GeneratorContext context, string? category)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(context),
            CreateConstructor(context, category)
        };
        if (category == null)
        {
            members.AddRange(CreateCategoryProperties(context));
        }
        members.AddRange(CreateRegisterProperties(context, category));

        return ClassDeclaration(GetRegistersClassName(context, category))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateCategoryProperties(GeneratorContext context) => GetCategories(context).Select(c => CreateGetOnlyProperty(context, GetRegistersClassName(context, c), c));

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateRegisterProperties(GeneratorContext context, string? category) =>
        context.Configuration.Registers.Values
            .Where(r => r.HasRegisterClassProperty && r.Category == category)
            .OrderBy(r => r.Name)
            .Select(r => CreateRegisterProperty(context, r));

    [Pure]
    private static PropertyDeclarationSyntax CreateRegisterProperty(GeneratorContext context, Register register)
    {
        var memberAccessExpression = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(register.FieldName));

        var getExpression = memberAccessExpression;

        var setExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            memberAccessExpression,
            IdentifierName("value"));

        return CreateGetSetProperty(context, register.Type.TypeSyntax(), register.PropertyName, getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context, string? category)
    {
        var statements = new List<StatementSyntax>
        {
            CreateAssignEmulatorFieldExpression()
        };

        if (category == null)
        {
            foreach (var c in GetCategories(context))
            {
                // Category = new Z80CategoryRegisters(emulator);
                statements.Add(CreateNewObjectAndAssignToProperty(c, GetRegistersClassName(context, c), IdentifierName(EmulatorFieldName)));
            }
        }

        return ConstructorDeclaration(GetRegistersClassName(context, category))
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(context)))))
            .WithBody(Block(statements.ToArray()))
            .WithLeadingTrivia(NewlineComment);
    }

    [Pure]
    private static IEnumerable<string> GetCategories(GeneratorContext context) => context.Configuration.Registers.Values.Where(r => r.Category != null).Select(r => r.Category!).Distinct().OrderBy(c => c);
}