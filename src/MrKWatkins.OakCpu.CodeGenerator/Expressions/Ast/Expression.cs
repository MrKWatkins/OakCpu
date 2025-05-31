using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public abstract class Expression : AstNode
{
    public virtual Type Type => throw new NotSupportedException($"Expressions of type {GetType().Name} do not have a type.");

    public virtual TypeSyntax TypeSyntax => throw new NotSupportedException($"Expressions of type {GetType().Name} do not have a type syntax.");
}