using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Provides the canonical identifiers used by generated types and members.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Identifiers
{
    /// <summary>
    /// Provides generated class identifiers.
    /// </summary>
    internal static class Class
    {
        /// <summary>
        /// Provides generated class names.
        /// </summary>
        internal static class Name
        {
            /// <summary>
            /// Gets the generated step-emulator class name.
            /// </summary>
            [Pure]
            public static string Emulator(GeneratorContext context) => $"{context.Cpu.Name}StepEmulator";

            /// <summary>
            /// Gets the generated instruction-emulator class name.
            /// </summary>
            [Pure]
            public static string InstructionEmulator(GeneratorContext context) => $"{context.Cpu.Name}InstructionEmulator";

            /// <summary>
            /// Gets the generated registers facade class name.
            /// </summary>
            [Pure]
            public static string Registers(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}{category}Registers";

            /// <summary>
            /// Gets the generated step-emulator registers facade class name.
            /// </summary>
            [Pure]
            public static string StepRegisters(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}Step{category}Registers";

            /// <summary>
            /// Gets the generated instruction-emulator registers facade class name.
            /// </summary>
            [Pure]
            public static string InstructionRegisters(GeneratorContext context, string? category = null) => $"{context.Cpu.Name}Instruction{category}Registers";

            /// <summary>
            /// Gets the generated flags facade class name.
            /// </summary>
            [Pure]
            public static string Flags(GeneratorContext context) => $"{context.Cpu.Name}Flags";

            /// <summary>
            /// Gets the generated step-emulator flags facade class name.
            /// </summary>
            [Pure]
            public static string StepFlags(GeneratorContext context) => $"{context.Cpu.Name}StepFlags";

            /// <summary>
            /// Gets the generated instruction-emulator flags facade class name.
            /// </summary>
            [Pure]
            public static string InstructionFlags(GeneratorContext context) => $"{context.Cpu.Name}InstructionFlags";

            /// <summary>
            /// Gets the generated interrupts facade class name.
            /// </summary>
            [Pure]
            public static string Interrupts(GeneratorContext context) => $"{context.Cpu.Name}Interrupts";

            /// <summary>
            /// Gets the generated step-emulator interrupts facade class name.
            /// </summary>
            [Pure]
            public static string StepInterrupts(GeneratorContext context) => $"{context.Cpu.Name}StepInterrupts";

            /// <summary>
            /// Gets the generated instruction-emulator interrupts facade class name.
            /// </summary>
            [Pure]
            public static string InstructionInterrupts(GeneratorContext context) => $"{context.Cpu.Name}InstructionInterrupts";
        }

        /// <summary>
        /// Provides generated class identifiers.
        /// </summary>
        internal static class Identifier
        {
            /// <summary>
            /// Gets an identifier for the generated step-emulator class.
            /// </summary>
            [Pure]
            public static IdentifierNameSyntax Emulator(GeneratorContext context) => IdentifierName(Name.Emulator(context));

            /// <summary>
            /// Gets an identifier for the generated instruction-emulator class.
            /// </summary>
            [Pure]
            public static IdentifierNameSyntax InstructionEmulator(GeneratorContext context) => IdentifierName(Name.InstructionEmulator(context));
        }
    }

    /// <summary>
    /// Provides generated field identifiers.
    /// </summary>
    internal static class Field
    {
        /// <summary>
        /// Provides generated field names.
        /// </summary>
        internal static class Name
        {
            /// <summary>
            /// The instruction handlers field.
            /// </summary>
            public const string Instructions = "Instructions";

            /// <summary>
            /// The overlap handlers field.
            /// </summary>
            public const string Overlaps = "Overlaps";

            /// <summary>
            /// The handler field on the generated step struct.
            /// </summary>
            public const string Handler = "Handler";

            /// <summary>
            /// The next-step field on the generated step struct.
            /// </summary>
            public const string NextStep = "NextStep";

            /// <summary>
            /// The action-required field on the generated step struct.
            /// </summary>
            public const string ActionRequired = "ActionRequired";

            /// <summary>
            /// The overlap field on the generated step struct.
            /// </summary>
            public const string Overlap = "Overlap";

            /// <summary>
            /// The deferred next-sequence-step field on the instruction emulator.
            /// </summary>
            public const string NextSequenceStep = "nextSequenceStep";

            /// <summary>
            /// The sentinel field name used when no next sequence is scheduled.
            /// </summary>
            public const string NoNextSequenceStep = "NoNextSequenceStep";

            /// <summary>
            /// Gets the sequence-group step table field name.
            /// </summary>
            [Pure]
            public static string SequenceGroupStepTable(SequenceGroup group) => $"{group.Name.ToUpperCamelCaseFromSnakeCase()}StepTable";
        }
    }

    /// <summary>
    /// Provides generated method identifiers.
    /// </summary>
    internal static class Method
    {
        /// <summary>
        /// Provides generated method names.
        /// </summary>
        internal static class Name
        {
            /// <summary>
            /// The error handler method.
            /// </summary>
            public const string Error = "Error";

            /// <summary>
            /// The method that completes an instruction.
            /// </summary>
            public const string CompleteInstruction = "CompleteInstruction";

            /// <summary>
            /// The interrupt-handling method.
            /// </summary>
            public const string HandleInterrupts = "HandleInterrupts";

            /// <summary>
            /// The overlap execution method.
            /// </summary>
            public const string ExecuteOverlap = "ExecuteOverlap";

            private const string OverlapPrefix = "Overlap";
            private const string StepPrefix = "Step";

            /// <summary>
            /// Gets the generated step handler method name.
            /// </summary>
            [Pure]
            public static string Step(Step step) =>
                step.MethodIndex != null
                    ? $"{StepPrefix}{step.MethodIndex}"
                    : throw new InvalidOperationException($"Step {step.Name} does not have a {nameof(step.MethodIndex)}.");

            /// <summary>
            /// Gets the generated overlap handler method name.
            /// </summary>
            [Pure]
            public static string Overlap(GeneratorContext context, Step step) => $"{OverlapPrefix}{context.GetOverlapMethodIndex(step)}";
        }
    }

    /// <summary>
    /// Provides generated parameter identifiers.
    /// </summary>
    internal static class Parameter
    {
        /// <summary>
        /// Provides generated parameter names.
        /// </summary>
        internal static class Name
        {
            /// <summary>
            /// The callback parameter that reports required external actions.
            /// </summary>
            public const string InstructionActionCallback = "onActionRequired";

            /// <summary>
            /// The emulator parameter name.
            /// </summary>
            public const string Emulator = "emulator";

            /// <summary>
            /// The action-required parameter name.
            /// </summary>
            public const string ActionRequired = "actionRequired";
        }

        /// <summary>
        /// Provides generated parameter syntax nodes.
        /// </summary>
        internal static class Syntax
        {
            /// <summary>
            /// Creates the parameter used by generated step-emulator instance helpers.
            /// </summary>
            [Pure]
            public static ParameterSyntax Emulator(GeneratorContext context) =>
                Parameter(Identifier(Name.Emulator)).WithType(Class.Identifier.Emulator(context));

            /// <summary>
            /// Creates the parameter used by generated static instruction-emulator handlers.
            /// </summary>
            [Pure]
            public static ParameterSyntax InstructionEmulator(GeneratorContext context) =>
                Parameter(Identifier(Name.Emulator)).WithType(Class.Identifier.InstructionEmulator(context));

            /// <summary>
            /// Creates the callback parameter used by generated instruction handlers to request external bus actions.
            /// </summary>
            [Pure]
            public static ParameterSyntax InstructionActionCallback() =>
                Parameter(Identifier(Name.InstructionActionCallback))
                    .WithType(
                        GenericName(Identifier("Action"))
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SeparatedList<TypeSyntax>(
                                    [
                                        IdentifierName(TypeName.ActionRequiredEnum),
                                        Token(SyntaxKind.CommaToken),
                                        UShortType,
                                        Token(SyntaxKind.CommaToken),
                                        ByteType
                                    ]))));
        }
    }

    /// <summary>
    /// Provides generated property identifiers.
    /// </summary>
    internal static class Property
    {
        /// <summary>
        /// Provides generated property names.
        /// </summary>
        internal static class Name
        {
            /// <summary>
            /// The generated property name for the register facade.
            /// </summary>
            public const string Registers = "Registers";

            /// <summary>
            /// The generated property name for the flags facade.
            /// </summary>
            public const string Flags = "Flags";

            /// <summary>
            /// The generated property name for the interrupts facade.
            /// </summary>
            public const string Interrupts = "Interrupts";
        }
    }

    /// <summary>
    /// Provides generated type identifiers that do not fit the class-name hierarchy.
    /// </summary>
    internal static class TypeName
    {
        /// <summary>
        /// The generated action-required enum name.
        /// </summary>
        public const string ActionRequiredEnum = "ActionRequired";

        /// <summary>
        /// The generated step struct name.
        /// </summary>
        public const string StepStruct = "Step";
    }
}