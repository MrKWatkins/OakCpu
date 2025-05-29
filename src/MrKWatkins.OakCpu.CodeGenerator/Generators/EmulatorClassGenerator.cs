using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : ClassGenerator
{
    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input) =>
        PopulateClass(requiredUsings, input, ClassDeclaration(GetEmulatorClassName(input)).AddModifiers(Public, Sealed, Partial));

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration);
}