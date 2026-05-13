using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : TypeGenerator
{
    protected const string StepsFieldName = "Steps";

    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        var classDeclaration = PopulateClass(
            context,
            ClassDeclaration(Class.Name.Emulator(context)).AddModifiers(Public, Sealed, Unsafe, Partial));

        return GetBaseFileName(context) == Class.Name.Emulator(context)
            ? WithXmlDocumentation(classDeclaration, $"Represents a cycle-accurate {context.GeneratorContext.Cpu.Name} emulator that executes one T-state at a time.")
            : classDeclaration;
    }

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(FileGeneratorContext context, ClassDeclarationSyntax classDeclaration);

}