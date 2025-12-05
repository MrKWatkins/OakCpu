namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal abstract class Optimizer
{
    private static readonly IReadOnlyList<Optimizer> All =
    [
        new SimplifyBinaryIdentityOperations(),
        new UnaryConstantFolding()
    ];

    [MustUseReturnValue]
    internal static T Optimize<T>(T node)
        where T : AstNode
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

        return (T)All.Aggregate<Optimizer, AstNode>(node, (current, optimizer) => optimizer.OptimizeNode(current));
    }

    [Pure]
    protected abstract AstNode OptimizeNode(AstNode node);
}

internal abstract class Optimizer<T> : Optimizer
    where T : AstNode
{
    protected override AstNode OptimizeNode(AstNode node) => node is T typedNode ? OptimizeNode(typedNode) : node;

    protected abstract AstNode OptimizeNode(T unaryOperation);
}