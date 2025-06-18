namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast.Optimization;

public static class OptimizeStatements
{
    [Pure]
    public static IEnumerable<Statement> Optimize([InstantHandle] IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            // Skip self-assignments, e.g. LD B, B.
            if (statement is Assignment assignment && assignment.Target == assignment.Value)
            {
                continue;
            }
            yield return statement;
        }
    }
}