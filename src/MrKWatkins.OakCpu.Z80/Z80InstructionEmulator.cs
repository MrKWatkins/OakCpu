using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MrKWatkins.OakCpu.Z80;

public sealed unsafe partial class Z80InstructionEmulator
{
    public int ExecuteInstruction(Action<ActionRequired, ushort, byte> onActionRequired)
    {
        ArgumentNullException.ThrowIfNull(onActionRequired);

        if (pendingInterruptStep != 0)
        {
            return ExecutePendingInterrupt(onActionRequired);
        }

        if (halted)
        {
            return ExecuteDecodedInstruction(HaltedStep0, onActionRequired);
        }

        var tStates = ReadOpcodeFromStart(onActionRequired);
        var decodedStep = DecodeInstructionFromData(onActionRequired, ref tStates);
        return tStates + ExecuteDecodedInstruction(decodedStep, onActionRequired);
    }

    private int ExecutePendingInterrupt(Action<ActionRequired, ushort, byte> onActionRequired)
    {
        if (pendingInterruptStep == 0)
        {
            return 0;
        }

        var interruptStep = pendingInterruptStep;
        pendingInterruptStep = 0;
        return ExecuteDecodedInstruction(interruptStep, onActionRequired);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CompleteInstruction(bool instructionUpdatesFlags, int tStates)
    {
        Q = instructionUpdatesFlags ? F : (byte)0;
        HandleInterrupts(this);
        return tStates;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void QueueInterrupt(ushort interruptStep)
    {
        pendingInterruptStep = interruptStep;
    }

    private int ReadOpcodeFromStart(Action<ActionRequired, ushort, byte> onActionRequired)
    {
        address = PC;
        PC += 0x01;
        onActionRequired(ActionRequired.OpcodeRead, address, data);
        address = IR;
        R = (byte)(R & 0b10000000 | R + 0x01 & 0b01111111);
        return 3;
    }

    private ushort DecodeInstructionFromData(Action<ActionRequired, ushort, byte> onActionRequired, ref int tStates)
    {
        var decodedStep = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(opcodeStepTable), data);
        tStates += 1;
        if (!TrySetPrefixOpcodeStepTable(decodedStep))
        {
            return decodedStep;
        }

        tStates += ReadOpcodeFromStart(onActionRequired);
        return DecodeInstructionFromData(onActionRequired, ref tStates);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ExecuteDecodedInstruction(ushort decodedStep, Action<ActionRequired, ushort, byte> onActionRequired)
    {
        var instruction = Instructions[decodedStep];
        return instruction(this, onActionRequired);
    }
}