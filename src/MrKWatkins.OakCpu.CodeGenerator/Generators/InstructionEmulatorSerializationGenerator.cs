using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorSerializationGenerator : SerializationGenerator
{
    public static readonly InstructionEmulatorSerializationGenerator Instance = new();

    private InstructionEmulatorSerializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetInstructionEmulatorClassName(context)}.serialization";

    [Pure]
    protected override string GetSerializedTypeName(GeneratorContext context) => GetInstructionEmulatorClassName(context);

    [Pure]
    protected override IEnumerable<DataMember> GetSerializedDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(member => member != PreDefinedDataMember.CurrentStep && member != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(member => member.Name);

    [Pure]
    protected override IEnumerable<StatementSyntax> GenerateAdditionalSerializeStatements(GeneratorContext context)
    {
        yield return GenerateWrite(IdentifierName(NextSequenceStepFieldName));
    }

    [Pure]
    protected override IEnumerable<StatementSyntax> GenerateAdditionalRestoreStatements(GeneratorContext context)
    {
        yield return GenerateRead(NextSequenceStepFieldName, DataType.U16);
    }
}