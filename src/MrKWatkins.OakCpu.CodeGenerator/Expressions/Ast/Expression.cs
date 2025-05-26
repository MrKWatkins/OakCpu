using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

// Not using MrKWatkins.Ast as it does not (currently) support .NET Standard 2.0.
public abstract class Expression
{
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        WriteExpression(stringBuilder);
        return stringBuilder.ToString();
    }

    public virtual Type Type => throw new NotSupportedException($"Expressions of type {GetType().Name} do not have a type.");

    public virtual TypeSyntax TypeSyntax => throw new NotSupportedException($"Expressions of type {GetType().Name} do not have a type syntax.");

    public virtual IEnumerable<Expression> Children => [];

    public abstract void WriteExpression(StringBuilder expression);
}