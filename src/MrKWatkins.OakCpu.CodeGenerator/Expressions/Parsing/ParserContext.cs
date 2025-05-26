using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

public sealed class ParserContext(IReadOnlyCollection<string> actions, IReadOnlyDictionary<string, Register> registers, IReadOnlyDictionary<string, Flag> flags)
{
    public IReadOnlyCollection<string> Actions { get; } = actions;

    public IReadOnlyDictionary<string, Register> Registers { get; } = registers;

    public IReadOnlyDictionary<string, Flag> Flags { get; } = flags;
}