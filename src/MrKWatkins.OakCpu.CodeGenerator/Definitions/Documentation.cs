namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed record Documentation(string Summary)
{
    public static Documentation Empty { get; } = new("");

    public bool IsEmpty => Summary.Length == 0;

    [Pure]
    public static Documentation Create(string? yaml, string target)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            throw new InvalidOperationException($"Documentation is missing for {target}.");
        }

        return new Documentation(yaml);
    }

    [Pure]
    public static Documentation CreateOptional(string? yaml, string target) =>
        string.IsNullOrWhiteSpace(yaml) ? Empty : Create(yaml, target);
}