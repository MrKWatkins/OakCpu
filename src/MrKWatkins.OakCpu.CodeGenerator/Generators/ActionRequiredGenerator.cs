using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class ActionRequiredGenerator : ClassGenerator
{
    public static readonly ActionRequiredGenerator Instance = new();

    private ActionRequiredGenerator()
    {
    }

    protected override BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input)
    {
        var members = input.Cpu.Actions.Values
            .OrderBy(a => a.Value)
            .Select(action => EnumMemberDeclaration([], Identifier(action.EnumName), EqualsValueClause(GenerateNumericLiteralExpression(action.Value))))
            .ToArray();

        return EnumDeclaration(ActionRequiredEnumName)
            .AddModifiers(Public)
            .AddMembers(members);
    }
}