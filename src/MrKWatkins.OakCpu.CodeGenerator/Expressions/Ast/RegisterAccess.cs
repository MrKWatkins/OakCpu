using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class RegisterAccess(Register register) : Expression
{
    public Register Register { get; } = register;

    public override Type Type => Register.Type;

    public override TypeSyntax TypeSyntax => Register.TypeSyntax;

    public IdentifierNameSyntax IdentifierName => SyntaxFactory.IdentifierName(Register.Name);

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append("Register.");
        expression.Append(Register.Name);
    }
}