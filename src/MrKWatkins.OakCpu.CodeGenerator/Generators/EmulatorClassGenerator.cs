using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : ClassGenerator
{
    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override ClassDeclarationSyntax CreateClass(HashSet<string> requiredUsings, GeneratorInput input) =>
        PopulateClass(requiredUsings, input, SyntaxFactory.ClassDeclaration(EmulatorClassName).AddModifiers(Public, Sealed, Partial));

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration);
}