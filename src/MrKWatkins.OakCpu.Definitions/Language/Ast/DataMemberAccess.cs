
namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class DataMemberAccess(DataMember dataMember) : Access(dataMember.FieldName)
{
    public DataMember DataMember { get; } = dataMember;

    public override DataType Type => DataMember.Type;
}