using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the bus-handler interface implemented by callers of the instruction emulator. A concrete struct handler
/// passed by <c>ref</c> lets the JIT monomorphise and inline the bus callback, replacing the per-cycle delegate
/// invocation that the previous <c>Action&lt;ActionRequired, ushort, byte&gt;</c> callback required.
/// </summary>
public sealed class InstructionEmulatorBusHandlerGenerator : TypeGenerator
{
    public static readonly InstructionEmulatorBusHandlerGenerator Instance = new();

    private InstructionEmulatorBusHandlerGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.InstructionBusHandler(context);

    protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        var method = WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(InstructionHandlerSyntax.OnActionRequiredMethodName))
                .WithParameterList(
                    ParameterList(
                    [
                        Parameter(Identifier("actionRequired")).WithType(IdentifierName(TypeName.ActionRequiredEnum)),
                        Parameter(Identifier("address")).WithType(UShortType),
                        Parameter(Identifier("data")).WithType(ByteType)
                    ]))
                .WithSemicolonToken(Semicolon),
            "Performs the external bus action required for the current CPU cycle.",
            parameters: new Dictionary<string, string>
            {
                ["actionRequired"] = "The action the host must perform.",
                ["address"] = "The address on the external bus.",
                ["data"] = "The data on the external bus for write actions; ignored for reads."
            });

        return WithXmlDocumentation(
            InterfaceDeclaration(Class.Name.InstructionBusHandler(context))
                .AddModifiers(Public)
                .AddMembers(method),
            $"Handles the external bus actions requested by the {Class.Name.InstructionEmulator(context)}.");
    }
}