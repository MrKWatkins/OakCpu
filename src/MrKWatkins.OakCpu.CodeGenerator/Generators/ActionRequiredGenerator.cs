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
        var members = input.Cpu.Actions.Prepend(ActionRequiredNone).Select(name => SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(name))).ToArray();

        return SyntaxFactory.EnumDeclaration(ActionRequiredEnumName)
            .AddModifiers(Public)
            .AddMembers(members);
    }
}