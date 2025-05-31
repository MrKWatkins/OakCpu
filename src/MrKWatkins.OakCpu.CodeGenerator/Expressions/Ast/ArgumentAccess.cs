using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class ArgumentAccess(string name) : Expression
{
    public string Name { get; } = name;

    public IdentifierNameSyntax IdentifierName => SyntaxFactory.IdentifierName(Name);

    public override void WriteExpression(StringBuilder expression) => expression.Append(Name);
}