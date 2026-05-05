using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

// Not using MrKWatkins.Ast because this AST supports in-place child replacement during later binding and rewriting passes.
// Switching to MrKWatkins.Ast would still require a non-trivial rework because nodes are reused and if statements would need a different representation.
public abstract class AstNode
{
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        WriteStringRepresentation(stringBuilder);
        return stringBuilder.ToString();
    }

    public virtual IEnumerable<AstNode> Children => [];

    public virtual void ReplaceChild(AstNode original, AstNode replacement) => throw new InvalidOperationException($"{GetType().Name} nodes do do not support replacing children.");

    public abstract void WriteStringRepresentation(StringBuilder stringRepresentation);

    [Pure]
    public IEnumerable<AstNode> TraverseDepthFirst()
    {
        foreach (var descendent in Children.SelectMany(c => c.TraverseDepthFirst()))
        {
            yield return descendent;
        }
        yield return this;
    }
}