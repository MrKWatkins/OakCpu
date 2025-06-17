using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : TypeGenerator
{
    protected const string StepsFieldName = "Steps";

    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(GeneratorContext context) =>
        PopulateClass(context, ClassDeclaration(GetEmulatorClassName(context)).AddModifiers(Public, Sealed, Partial));

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration);

    [Pure]
    protected static ParameterSyntax CreateEmulatorParameter(GeneratorContext context) => Parameter(Identifier(EmulatorParameterName)).WithType(IdentifierName(GetEmulatorClassName(context)));
}