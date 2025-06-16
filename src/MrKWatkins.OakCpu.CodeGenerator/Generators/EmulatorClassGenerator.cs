using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : ClassGenerator
{
    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(GeneratorContext context) =>
        PopulateClass(context, ClassDeclaration(GetEmulatorClassName(context)).AddModifiers(Public, Sealed, Partial));

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration);
}