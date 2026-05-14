namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

internal abstract class Optimizer
{
    private static readonly IReadOnlyList<Optimizer> All =
    [
        new SimplifySelfBitwiseBinaryOperations(),
        new SimplifyBinaryIdentityOperations(),
        new BinaryConstantFolding(),
        new UnaryConstantFolding()
    ];
    private static readonly IReadOnlyDictionary<Type, IReadOnlyList<Optimizer>> ByNodeType = All
        .GroupBy(static optimizer => optimizer.NodeType)
        .ToDictionary(static group => group.Key, static group => (IReadOnlyList<Optimizer>)group.ToArray());

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

        if (!ByNodeType.TryGetValue(node.GetType(), out var optimizers))
        {
            return node;
        }

        return (T)optimizers.Aggregate<Optimizer, AstNode>(node, static (current, optimizer) => optimizer.OptimizeNode(current));
    }

    protected abstract Type NodeType { get; }

    [Pure]
    protected abstract AstNode OptimizeNode(AstNode node);
}

internal abstract class Optimizer<T> : Optimizer
    where T : AstNode
{
    protected override Type NodeType => typeof(T);

    protected override AstNode OptimizeNode(AstNode node) => node is T typedNode ? OptimizeNode(typedNode) : node;

    protected abstract AstNode OptimizeNode(T unaryOperation);
}