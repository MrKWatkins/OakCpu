using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : TypeGenerator
{
    protected const string StepsFieldName = "Steps";

    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var classDeclaration = PopulateClass(
            context,
            ClassDeclaration(GetEmulatorClassName(context)).AddModifiers(Public, Sealed, Unsafe, Partial));

        return GetBaseFileName(context) == GetEmulatorClassName(context)
            ? WithXmlDocumentation(classDeclaration, $"Represents a cycle-accurate {context.Cpu.Name} emulator that executes one T-state at a time.")
            : classDeclaration;
    }

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration);

    [Pure]
    protected static ParameterSyntax CreateEmulatorParameter(GeneratorContext context) => Parameter(Identifier(EmulatorParameterName)).WithType(IdentifierName(GetEmulatorClassName(context)));
}