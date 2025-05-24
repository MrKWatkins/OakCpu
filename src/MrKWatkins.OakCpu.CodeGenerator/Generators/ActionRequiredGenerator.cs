using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class ActionRequiredGenerator : ClassGenerator
{
    public static readonly ActionRequiredGenerator Instance = new();

    private ActionRequiredGenerator()
    {
    }

    protected override BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input)
    {
        var members = ActionRequired.Members.Select(name => SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(name))).ToArray();

        return SyntaxFactory.EnumDeclaration(ActionRequired.EnumName)
            .AddModifiers(Public)
            .AddMembers(members);
    }
}