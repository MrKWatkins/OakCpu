using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class ActionRequiredGenerator : TypeGenerator
{
    public static readonly ActionRequiredGenerator Instance = new();

    private ActionRequiredGenerator()
    {
    }

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = context.Configuration.Actions.Values
            .OrderBy(a => a.Value)
            .Select(action => EnumMemberDeclaration([], Identifier(action.EnumName), EqualsValueClause(GenerateNumericLiteralExpression(action.Value))))
            .ToArray();

        return EnumDeclaration(ActionRequiredEnumName)
            .AddModifiers(Public)
            .AddMembers(members);
    }
}