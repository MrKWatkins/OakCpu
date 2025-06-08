using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public abstract class Expression : AstNode
{
    public abstract DataType Type { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax();
}