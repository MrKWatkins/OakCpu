namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

public sealed class ParserContext(Configuration configuration)
{
    public Configuration Configuration { get; } = configuration;

    public IReadOnlyCollection<string> Arguments { get; private set; } = [];

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) =>
        new(Configuration)
        {
            Arguments = arguments
        };
}