using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

// Not using MrKWatkins.Ast as it does not (currently) support .NET Standard 2.0. Although now we scrapped the source generator, this no longer applies.
// Not trivial to change now either as nodes are reused and if statement would require a rework.
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