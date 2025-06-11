using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class DataMemberAccess(DataMember dataMember) : Access(dataMember.Name)
{
    public DataMember DataMember { get; } = dataMember;

    public override IdentifierNameSyntax Identifier => SyntaxFactory.IdentifierName(DataMember.MemberName);

    public override DataType Type => DataMember.Type;
}