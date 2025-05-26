using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class DataMemberAccess(KnownDataMember dataMember) : Expression
{
    public KnownDataMember DataMember { get; } = dataMember;

    public override Type Type => DataMember.Type;

    public override TypeSyntax TypeSyntax => DataMember.TypeSyntax;

    public IdentifierNameSyntax IdentifierName => SyntaxFactory.IdentifierName(DataMember.Name);

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append("Member.");
        expression.Append(DataMember.Name);
    }
}