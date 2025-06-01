using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class DataMemberAccess(DataMember dataMember) : Access(dataMember.Name)
{
    public DataMember DataMember { get; } = dataMember;

    public override Type Type => DataMember.Type;

    public override TypeSyntax TypeSyntax => DataMember.TypeSyntax;
}