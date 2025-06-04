using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

public sealed class ParserContext(IReadOnlyCollection<string> actions, IReadOnlyDictionary<string, Register> registers, IReadOnlyDictionary<string, Flag> flags)
{
    public IReadOnlyCollection<string> Actions { get; } = actions;

    public IReadOnlyDictionary<string, Register> Registers { get; } = registers;

    public IReadOnlyDictionary<string, Flag> Flags { get; } = flags;

    public IReadOnlyDictionary<string, UserDefinedFunction> UserDefinedFunctions { get; private set; } = new Dictionary<string, UserDefinedFunction>();

    public IReadOnlyCollection<string> Parameters { get; private set; } = [];

    [Pure]
    public ParserContext WithFunctions(IReadOnlyDictionary<string, UserDefinedFunction> functions) =>
        new(Actions, Registers, Flags)
        {
            UserDefinedFunctions = functions
        };

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) =>
        new(Actions, Registers, Flags)
        {
            UserDefinedFunctions = UserDefinedFunctions,
            Parameters = arguments
        };
}