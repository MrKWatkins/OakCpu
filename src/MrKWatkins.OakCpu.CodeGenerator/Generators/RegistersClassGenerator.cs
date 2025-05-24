using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class RegistersClassGenerator : ClassGenerator
{
    public static readonly RegistersClassGenerator Instance = new();

    private RegistersClassGenerator()
    {
    }

    protected override ClassDeclarationSyntax CreateClass(HashSet<string> requiredUsings, GeneratorInput input)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField()
        };

        return SyntaxFactory
            .ClassDeclaration("Z80Registers")
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static FieldDeclarationSyntax CreateEmulatorField() =>
        SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("Z80Emulator"))
                    .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("emulator")))))
            .WithModifiers(SyntaxFactory.TokenList(Private, ReadOnly));
}