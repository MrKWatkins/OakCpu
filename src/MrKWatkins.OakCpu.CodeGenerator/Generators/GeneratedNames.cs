using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class GeneratedNames
{
    [Pure]
    public static string GetEmulatorClassName(GeneratorContext context) => $"{context.Cpu.Name}StepEmulator";

    [Pure]
    public static IdentifierNameSyntax GetEmulatorClassIdentifier(GeneratorContext context) => IdentifierName(GetEmulatorClassName(context));

    [Pure]
    public static string GetInstructionEmulatorClassName(GeneratorContext context) => $"{context.Cpu.Name}InstructionEmulator";

    [Pure]
    public static IdentifierNameSyntax GetInstructionEmulatorClassIdentifier(GeneratorContext context) => IdentifierName(GetInstructionEmulatorClassName(context));

    [Pure]
    public static ParameterSyntax CreateInstructionEmulatorParameter(GeneratorContext context) => Parameter(Identifier("emulator")).WithType(GetInstructionEmulatorClassIdentifier(context));

    [Pure]
    public static string GetRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}{category}Registers";

    [Pure]
    public static string GetStepRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}Step{category}Registers";

    [Pure]
    public static string GetInstructionRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}Instruction{category}Registers";

    [Pure]
    public static string GetFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}Flags";

    [Pure]
    public static string GetStepFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}StepFlags";

    [Pure]
    public static string GetInstructionFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}InstructionFlags";

    [Pure]
    public static string GetInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}Interrupts";

    [Pure]
    public static string GetStepInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}StepInterrupts";

    [Pure]
    public static string GetInstructionInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}InstructionInterrupts";
}