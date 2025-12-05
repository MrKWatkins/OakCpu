namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal abstract class Optimizer
{
    private static readonly IReadOnlyList<Optimizer> All =
    [
        new SimplifyBinaryIdentityOperations()
    ];

    [MustUseReturnValue]
    internal static T Optimize<T>(T root)
        where T : AstNode =>
        All.Aggregate(root, (current, optimization) => (T)optimization.OptimizeNodeAndChildren(current));

    [MustUseReturnValue]
    private AstNode OptimizeNodeAndChildren(AstNode node)
    {
        var children = node.Children.ToArray();
        foreach (var child in children)
        {
            var optimized = Optimize(child);
            if (optimized != child)
            {
                node.ReplaceChild(child, optimized);
            }
        }

        return OptimizeNode(node);
    }

    [Pure]
    protected abstract AstNode OptimizeNode(AstNode node);
}

internal abstract class Optimizer<T> : Optimizer
    where T : AstNode
{
    protected override AstNode OptimizeNode(AstNode node) => node is T typedNode ? OptimizeNode(typedNode) : node;

    protected abstract AstNode OptimizeNode(T node);
}