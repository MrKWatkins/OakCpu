using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the reset method for the instruction emulator.
/// </summary>
public sealed class InstructionEmulatorResetGenerator : TypeGenerator
{
    private const string ResetMethodName = "Reset";

    /// <summary>
    /// The singleton instance of the generator.
    /// </summary>
    public static readonly InstructionEmulatorResetGenerator Instance = new();

    private InstructionEmulatorResetGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.InstructionEmulator(context)}.reset";

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context) =>
        ClassDeclaration(Class.Name.InstructionEmulator(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddMembers(GenerateReset(context));

    [Pure]
    private static MemberDeclarationSyntax GenerateReset(GeneratorContext context)
    {
        var statements = ResetSyntax.GenerateResetOpcodeStepTable(context)
            .Concat(GenerateResetDataMembers(context))
            .Append(GenerateResetNextSequenceStep())
            .Concat(ResetSyntax.GenerateResetRegisters(context));

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(ResetMethodName))
                .WithModifiers([Public])
                .WithBody(Block(statements)),
            $"Resets the {context.Cpu.Name} CPU state.");
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateResetDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable && m != PreDefinedDataMember.CurrentStep)
            .OrderBy(m => m.Name)
            .Select(m => ResetSyntax.GenerateReset(m.FieldName, m.Type));

    [Pure]
    private static StatementSyntax GenerateResetNextSequenceStep() =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(Field.Name.NextSequenceStep),
                IdentifierName(Field.Name.NoNextSequenceStep)));

}