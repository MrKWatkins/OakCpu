namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class PreDefinedFunction : Function
{
    public static readonly PreDefinedFunction CopyFrom = new("copy_from", DataType.U8, ["value"]);
    public static readonly PreDefinedFunction InstructionComplete = new("instruction_complete", DataType.Void, ["value"]);
    public static readonly PreDefinedFunction Flags = new("flags", DataType.Void, []);
    public static readonly PreDefinedFunction Handled = new("handled", DataType.Void, []);
    public static readonly PreDefinedFunction HandleInterrupts = new("handle_interrupts", DataType.Void, []);
    public static readonly PreDefinedFunction InstructionUpdatesFlags = new("instruction_updates_flags", DataType.Bool, []);
    // TODO: Should this be is_byte_zero? I.e. only check the value casted to a byte rather than the full int value.
    public static readonly PreDefinedFunction IsZero = new("is_zero", DataType.Bool, ["value"]);
    public static readonly PreDefinedFunction MoveToInterruptMode = new("move_to_interrupt_mode", DataType.Void, ["mode"]);
    public static readonly PreDefinedFunction MoveToOpcode = new("move_to_opcode", DataType.Void, []);
    public static readonly PreDefinedFunction MoveToSequence = new("move_to_sequence", DataType.Void, ["sequence"]);
    public static readonly PreDefinedFunction MoveToSequenceGroup = new("move_to_sequence_group", DataType.Void, ["group", "number"]);
    public static readonly PreDefinedFunction PopCount = new("pop_count", DataType.U8, ["value"]);
    public static readonly PreDefinedFunction Request = new("request", DataType.Void, ["action"]);
    public static readonly PreDefinedFunction SetOpcodeStepTable = new("set_opcode_step_table", DataType.Void, []);
    public static readonly PreDefinedFunction Signed = new("signed", DataType.I8, ["value"]);

    public static readonly IReadOnlyDictionary<string, PreDefinedFunction> All = new Dictionary<string, PreDefinedFunction>(StringComparer.OrdinalIgnoreCase)
    {
        { CopyFrom.Name, CopyFrom },
        { Flags.Name, Flags },
        { Handled.Name, Handled },
        { HandleInterrupts.Name, HandleInterrupts },
        { InstructionComplete.Name, InstructionComplete },
        { InstructionUpdatesFlags.Name, InstructionUpdatesFlags },
        { IsZero.Name, IsZero },
        { MoveToInterruptMode.Name, MoveToInterruptMode },
        { MoveToOpcode.Name, MoveToOpcode },
        { MoveToSequence.Name, MoveToSequence },
        { MoveToSequenceGroup.Name, MoveToSequenceGroup },
        { PopCount.Name, PopCount },
        { Request.Name, Request },
        { SetOpcodeStepTable.Name, SetOpcodeStepTable },
        { Signed.Name, Signed }
    };

    private PreDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)}";
}
