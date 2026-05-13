using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Provides the canonical names used by generated types, properties, and helper parameters.
/// </summary>
internal static class GeneratedNames
{
    /// <summary>
    /// The generated property name for the register facade.
    /// </summary>
    public const string RegistersPropertyName = "Registers";

    /// <summary>
    /// The generated property name for the flags facade.
    /// </summary>
    public const string FlagsPropertyName = "Flags";

    /// <summary>
    /// The generated property name for the interrupts facade.
    /// </summary>
    public const string InterruptsPropertyName = "Interrupts";

    /// <summary>
    /// Gets the generated step-emulator class name.
    /// </summary>
    [Pure]
    public static string GetEmulatorClassName(GeneratorContext context) => $"{context.Cpu.Name}StepEmulator";

    /// <summary>
    /// Gets an identifier for the generated step-emulator class.
    /// </summary>
    [Pure]
    public static IdentifierNameSyntax GetEmulatorClassIdentifier(GeneratorContext context) => IdentifierName(GetEmulatorClassName(context));

    /// <summary>
    /// Gets the generated instruction-emulator class name.
    /// </summary>
    [Pure]
    public static string GetInstructionEmulatorClassName(GeneratorContext context) => $"{context.Cpu.Name}InstructionEmulator";

    /// <summary>
    /// Gets an identifier for the generated instruction-emulator class.
    /// </summary>
    [Pure]
    public static IdentifierNameSyntax GetInstructionEmulatorClassIdentifier(GeneratorContext context) => IdentifierName(GetInstructionEmulatorClassName(context));

    /// <summary>
    /// Creates the instruction-emulator parameter used by generated static handlers.
    /// </summary>
    [Pure]
    public static ParameterSyntax CreateInstructionEmulatorParameter(GeneratorContext context) => Parameter(Identifier("emulator")).WithType(GetInstructionEmulatorClassIdentifier(context));

    /// <summary>
    /// Gets the generated registers facade class name.
    /// </summary>
    [Pure]
    public static string GetRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}{category}Registers";

    /// <summary>
    /// Gets the generated step-emulator registers facade class name.
    /// </summary>
    [Pure]
    public static string GetStepRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}Step{category}Registers";

    /// <summary>
    /// Gets the generated instruction-emulator registers facade class name.
    /// </summary>
    [Pure]
    public static string GetInstructionRegistersClassName(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}Instruction{category}Registers";

    /// <summary>
    /// Gets the generated flags facade class name.
    /// </summary>
    [Pure]
    public static string GetFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}Flags";

    /// <summary>
    /// Gets the generated step-emulator flags facade class name.
    /// </summary>
    [Pure]
    public static string GetStepFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}StepFlags";

    /// <summary>
    /// Gets the generated instruction-emulator flags facade class name.
    /// </summary>
    [Pure]
    public static string GetInstructionFlagsClassName(GeneratorContext context) => $"{context.Cpu.Name}InstructionFlags";

    /// <summary>
    /// Gets the generated interrupts facade class name.
    /// </summary>
    [Pure]
    public static string GetInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}Interrupts";

    /// <summary>
    /// Gets the generated step-emulator interrupts facade class name.
    /// </summary>
    [Pure]
    public static string GetStepInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}StepInterrupts";

    /// <summary>
    /// Gets the generated instruction-emulator interrupts facade class name.
    /// </summary>
    [Pure]
    public static string GetInstructionInterruptsClassName(GeneratorContext context) => $"{context.Cpu.Name}InstructionInterrupts";
}