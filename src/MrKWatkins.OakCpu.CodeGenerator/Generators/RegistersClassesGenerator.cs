using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class RegistersClassesGenerator : ClassGenerator
{
    public static readonly RegistersClassesGenerator Instance = new();

    private RegistersClassesGenerator()
    {
    }

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(HashSet<string> requiredUsings, GeneratorInput input)
    {
        yield return CreateClass(input, null);

        foreach (var category in GetCategories(input))
        {
            yield return CreateClass(input, category);
        }
    }

    private static ClassDeclarationSyntax CreateClass(GeneratorInput input, string? category)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(input),
            CreateConstructor(input, category)
        };
        if (category == null)
        {
            members.AddRange(CreateCategoryProperties(input));
        }
        members.AddRange(CreateRegisterProperties(input, category));

        return ClassDeclaration(GetRegistersClassName(input, category))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateCategoryProperties(GeneratorInput input) => GetCategories(input).Select(c => CreateGetOnlyProperty(GetRegistersClassName(input, c), c));

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateRegisterProperties(GeneratorInput input, string? category) => input.Registers.Where(r => r.Category == category).OrderBy(r => r.Name).Select(CreateRegisterProperty);

    [Pure]
    private static PropertyDeclarationSyntax CreateRegisterProperty(Register register)
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

        return CreateGetSetProperty(register.DataType.TypeSyntax(), register.PropertyName, getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input, string? category)
    {
        var statements = new List<StatementSyntax>
        {
            CreateAssignEmulatorFieldExpression()
        };

        if (category == null)
        {
            foreach (var c in GetCategories(input))
            {
                // Category = new Z80CategoryRegisters(emulator);
                statements.Add(CreateNewObjectAndAssignToProperty(c, GetRegistersClassName(input, c), IdentifierName(EmulatorFieldName)));
            }
        }

        return ConstructorDeclaration(GetRegistersClassName(input, category))
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(input)))))
            .WithBody(Block(statements.ToArray()));
    }

    [Pure]
    private static IEnumerable<string> GetCategories(GeneratorInput input) => input.Registers.Where(r => r.Category != null).Select(r => r.Category!).Distinct().OrderBy(c => c);
}