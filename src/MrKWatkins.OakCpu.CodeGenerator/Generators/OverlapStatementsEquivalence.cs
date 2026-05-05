using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class OverlapStatementsEquivalence
{
    [Pure]
    public static bool AreEquivalent(IReadOnlyList<StatementSyntax> x, IReadOnlyList<StatementSyntax> y) =>
        x.Count == y.Count && x.Zip(y, (left, right) => left.IsEquivalentTo(right)).All(equal => equal);
}