using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public abstract class Access(string name) : Expression
{
    public string Name { get; } = name;

    public virtual IdentifierNameSyntax Identifier => SyntaxFactory.IdentifierName(Name);

    public sealed override void WriteExpression(StringBuilder expression) => expression.Append(Name);
}