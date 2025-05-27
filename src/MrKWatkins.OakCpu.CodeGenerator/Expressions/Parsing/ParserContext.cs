using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;

public sealed class ParserContext(IReadOnlyCollection<string> actions, IReadOnlyDictionary<string, Register> registers)
{
    public IReadOnlyCollection<string> Actions { get; } = actions;

    public IReadOnlyDictionary<string, Register> Registers { get; } = registers;
}