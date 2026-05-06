namespace MrKWatkins.OakCpu.CodeGenerator.Validation;

internal sealed record ValidationError(string Message, IReadOnlyList<string> Paths)
{
    public ValidationError(string message)
        : this(message, [])
    {
    }

    public ValidationError(string message, string path)
        : this(message, [path])
    {
    }

    public override string ToString() =>
        Paths.Count switch
        {
            0 => Message,
            1 => $"{Paths[0]}: {Message}",
            _ => $"{string.Join(", ", Paths)}: {Message}"
        };
}