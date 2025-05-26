using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class RegisterAccess(Register register) : Expression
{
    public Register Register { get; } = register;

    public override void WriteExpression(StringBuilder expression)
    {
        expression.Append("Register.");
        expression.Append(Register.Name);
    }
}