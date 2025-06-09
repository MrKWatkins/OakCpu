namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public abstract class Statement : AstNode
{
    public virtual bool IsTerminal => false;
}