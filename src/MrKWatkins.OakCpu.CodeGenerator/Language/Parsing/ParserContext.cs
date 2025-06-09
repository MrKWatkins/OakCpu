using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;

public sealed class ParserContext(IReadOnlyDictionary<string, Action> actions, IReadOnlyDictionary<string, Register> registers, IReadOnlyDictionary<string, Flag> flags, IReadOnlyDictionary<string, Condition> conditions)
{
    public IReadOnlyDictionary<string, Action> Actions { get; } = actions;

    public IReadOnlyDictionary<string, Register> Registers { get; } = registers;

    public IReadOnlyDictionary<string, Flag> Flags { get; } = flags;

    public IReadOnlyDictionary<string, Condition> Conditions { get; } = conditions;

    public OpcodeStepTables? OpcodeStepTables { get; private set; }

    public IReadOnlyDictionary<string, UserDefinedFunction> UserDefinedFunctions { get; private set; } = new Dictionary<string, UserDefinedFunction>();

    public IReadOnlyCollection<string> Parameters { get; private set; } = [];

    [Pure]
    public ParserContext WithOpcodeStepTables(OpcodeStepTables opcodeStepTables) =>
        new(Actions, Registers, Flags, Conditions)
        {
            UserDefinedFunctions = UserDefinedFunctions,
            OpcodeStepTables = opcodeStepTables,
            Parameters = Parameters
        };

    [Pure]
    public ParserContext WithFunctions(IReadOnlyDictionary<string, UserDefinedFunction> functions) =>
        new(Actions, Registers, Flags, Conditions)
        {
            UserDefinedFunctions = functions,
            OpcodeStepTables = OpcodeStepTables,
            Parameters = Parameters
        };

    [Pure]
    public ParserContext WithArguments(IReadOnlyCollection<string> arguments) =>
        new(Actions, Registers, Flags, Conditions)
        {
            UserDefinedFunctions = UserDefinedFunctions,
            OpcodeStepTables = OpcodeStepTables,
            Parameters = arguments
        };
}