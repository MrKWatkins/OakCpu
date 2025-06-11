using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator;

[SuppressMessage("ReSharper", "ParameterHidesMember")]
public sealed class Configuration(IReadOnlyDictionary<string, Register> registers, IReadOnlyDictionary<string, Flag> flags, OpcodeStepTables opcodeStepTables)
{
    private IReadOnlyDictionary<string, Action>? actions;
    private IReadOnlyDictionary<string, DataMember>? allDataMembers;
    private IReadOnlyDictionary<string, Function>? allFunctions;
    private Register? flagsRegister;
    private Register? programCounter;
    private IReadOnlyDictionary<string, UserDefinedDataMember>? userDefinedDataMembers;
    private IReadOnlyDictionary<string, UserDefinedFunction>? userDefinedFunctions;

    public IReadOnlyDictionary<string, Action> Actions
    {
        get => actions ?? throw new InvalidOperationException($"{nameof(Actions)} not yet parsed.");
        internal set => actions = value;
    }

    public IReadOnlyDictionary<string, Register> Registers { get; } = registers;

    public IReadOnlyDictionary<string, Flag> Flags { get; } = flags;

    public IReadOnlyDictionary<string, Condition> Conditions { get; } = Condition.Create(flags.Values);

    public OpcodeStepTables OpcodeStepTables => opcodeStepTables;

    public IReadOnlyDictionary<string, UserDefinedDataMember> UserDefinedDataMembers
    {
        get => userDefinedDataMembers ?? throw new InvalidOperationException($"{nameof(UserDefinedDataMember)} not yet parsed.");
        internal set => userDefinedDataMembers = value;
    }

    public IReadOnlyDictionary<string, DataMember> AllDataMembers => allDataMembers ??= UserDefinedDataMembers.Values.Concat<DataMember>(PreDefinedDataMember.All.Values).ToDictionary(f => f.Name);

    public IReadOnlyDictionary<string, UserDefinedFunction> UserDefinedFunctions
    {
        get => userDefinedFunctions ?? throw new InvalidOperationException($"{nameof(UserDefinedFunctions)} not yet parsed.");
        internal set => userDefinedFunctions = value;
    }

    public IReadOnlyDictionary<string, Function> AllFunctions => allFunctions ??= UserDefinedFunctions.Values.Concat<Function>(PreDefinedFunction.All.Values).ToDictionary(f => f.Name);

    public Register FlagsRegister => flagsRegister ??= GetSingleRegister(r => r.Flags, "flags");

    public Register ProgramCounter => programCounter ??= GetSingleRegister(r => r.ProgramCounter, "program_counter");

    [Pure]
    private Register GetSingleRegister(Func<Register, bool> predicate, string field)
    {
        var registers = Registers.Values.Where(predicate).ToList();
        return registers.Count switch
        {
            0 => throw new InvalidOperationException($"No registers with {field} set."),
            > 1 => throw new InvalidOperationException($"Multiple registers with {field} set."),
            _ => registers[0]
        };
    }
}