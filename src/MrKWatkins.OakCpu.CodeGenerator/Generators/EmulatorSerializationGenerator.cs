using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorSerializationGenerator : SerializationGenerator
{
    private const string SerializeOverlapPipelineMethodName = "SerializeOverlapPipeline";
    private const string RestoreOverlapPipelineMethodName = "RestoreOverlapPipeline";

    public static readonly EmulatorSerializationGenerator Instance = new();

    private EmulatorSerializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.Emulator(context)}.serialization";

    [Pure]
    protected override string GetSerializedTypeName(GeneratorContext context) => Class.Name.Emulator(context);

    [Pure]
    protected override IEnumerable<DataMember> GetSerializedDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Concat<DataMember>([PreDefinedDataMember.OverlapPipeline])
            .Where(member => member != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(member => member.Name);

    [Pure]
    protected override StatementSyntax GenerateSerializeDataMember(GeneratorContext context, DataMember member) =>
        member == PreDefinedDataMember.OverlapPipeline
            ? GenerateWrite(InvocationExpression(IdentifierName(SerializeOverlapPipelineMethodName)))
            : base.GenerateSerializeDataMember(context, member);

    [Pure]
    protected override StatementSyntax GenerateRestoreDataMember(GeneratorContext context, DataMember member) =>
        member == PreDefinedDataMember.OverlapPipeline
            ? ExpressionStatement(
                InvocationExpression(IdentifierName(RestoreOverlapPipelineMethodName))
                    .WithArgumentList(ArgumentList([Argument(GenerateReadExpression(DataType.U16))])))
            : base.GenerateRestoreDataMember(context, member);
}