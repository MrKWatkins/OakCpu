using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratedNames;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the reset method for the step emulator.
/// </summary>
public sealed class EmulatorResetGenerator : EmulatorClassGenerator
{
    private const string ResetMethodName = "Reset";

    /// <summary>
    /// The singleton instance of the generator.
    /// </summary>
    public static readonly EmulatorResetGenerator Instance = new();

    private EmulatorResetGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetEmulatorClassName(context)}.reset";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(GenerateReset(context));

    [Pure]
    private static MemberDeclarationSyntax GenerateReset(GeneratorContext context)
    {
        var statements = ResetSyntax.GenerateResetOpcodeStepTable(context)
            .Concat(GenerateResetDataMembers(context))
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
            .Concat<DataMember>([PreDefinedDataMember.OverlapPipeline])
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(m => m.Name)
            .Select(m => m == PreDefinedDataMember.OverlapPipeline
                ? GenerateResetOverlapPipeline(context)
                : ResetSyntax.GenerateReset(m.FieldName, m.Type));

    [Pure]
    private static StatementSyntax GenerateResetOverlapPipeline(GeneratorContext context) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(PreDefinedDataMember.OverlapPipeline.FieldName),
                DefaultExpression(CreateOverlapHandlerType(context))));

}