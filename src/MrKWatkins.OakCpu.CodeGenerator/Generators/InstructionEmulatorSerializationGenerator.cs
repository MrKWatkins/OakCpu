using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorSerializationGenerator : SerializationGenerator
{
    public static readonly InstructionEmulatorSerializationGenerator Instance = new();

    private InstructionEmulatorSerializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.InstructionEmulator(context)}.serialization";

    [Pure]
    protected override string GetSerializedTypeName(GeneratorContext context) => Class.Name.InstructionEmulator(context);

    [Pure]
    protected override IEnumerable<DataMember> GetSerializedDataFields(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(member => member != PreDefinedDataMember.CurrentStep && member != PreDefinedDataMember.OpcodeStepTable)
            .OrderByDescending(member => member.Size);

    [Pure]
    protected override IEnumerable<SerializedField> GetAdditionalSerializedFields(GeneratorContext context, int nextFieldOffset)
    {
        yield return new SerializedField(Field.Name.NextSequenceStep, DataType.U16, nextFieldOffset, DataType.U16.Size());
    }

    [Pure]
    protected override IEnumerable<StatementSyntax> GetRestoreValidationStatements(GeneratorContext context)
    {
        // The next sequence step indexes the dispatch table without a bounds check, so a corrupt state must not be
        // allowed to restore an out-of-range value.
        yield return SyntaxFactory.ParseStatement(
            $$"""
              if ({{Field.Name.NextSequenceStep}} != {{Field.Name.NoNextSequenceStep}} && {{Field.Name.NextSequenceStep}} >= {{context.InstructionEmulatorDispatchCount}})
              {
                  throw new InvalidOperationException("Unknown next sequence step.");
              }
              """);
    }
}