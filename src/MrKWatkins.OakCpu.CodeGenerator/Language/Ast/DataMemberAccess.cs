using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class DataMemberAccess(DataMember dataMember) : Access(dataMember.Name)
{
    public DataMember DataMember { get; } = dataMember;

    public override DataType Type => DataMember.Type;
}