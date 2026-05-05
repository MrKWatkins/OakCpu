using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class ActionRequiredGenerator : TypeGenerator
{
    public static readonly ActionRequiredGenerator Instance = new();

    private ActionRequiredGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => ActionRequiredEnumName;

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = context.Configuration.Actions.Values
            .OrderBy(a => a.Value)
            .Select(action => WithXmlDocumentation(
                EnumMemberDeclaration([], Identifier(action.EnumName), EqualsValueClause(GenerateNumericLiteralExpression(action.Value))),
                action.Documentation))
            .ToArray();

        return WithXmlDocumentation(
            EnumDeclaration(ActionRequiredEnumName)
                .AddModifiers(Public)
                .AddMembers(members),
            "Describes the external action that the host must perform for the current CPU cycle.");
    }
}