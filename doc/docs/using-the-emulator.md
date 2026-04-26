# Using the Emulator

OakCpu's contains two Z80 emulators:

| Type                                                                                  | Use when                                                                                           |
|---------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------|
| [`Z80StepEmulator`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/index.md)               | You need cycle-by-cycle control over memory and I/O timing.                                        |
| [`Z80InstructionEmulator`](API/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/index.md) | You want to execute one complete instruction at a time while still observing each external action. |

## Install the Package

```bash
dotnet add package MrKWatkins.OakCpu.Z80
```

Then import the Z80 namespace:

```csharp
using MrKWatkins.OakCpu.Z80;
```

## Step Emulator

[`Z80StepEmulator`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/index.md) advances one T-state at a time. Each call to `Step()` returns an [`ActionRequired`](API/MrKWatkins.OakCpu.Z80/ActionRequired/index.md) value describing what the host must do for that cycle.

```csharp
var cpu = new Z80StepEmulator();
var memory = new byte[65536];

while (true)
{
    var action = cpu.Step();
    switch (action)
    {
        case ActionRequired.OpcodeRead:
        case ActionRequired.MemoryRead:
            cpu.Data = memory[cpu.Address];
            break;

        case ActionRequired.MemoryWrite:
            memory[cpu.Address] = cpu.Data;
            break;

        case ActionRequired.IORead:
            cpu.Data = ReadPort(cpu.Address);
            break;

        case ActionRequired.IOWrite:
            WritePort(cpu.Address, cpu.Data);
            break;
    }
}
```

The step emulator is the best fit for machine emulators where exact bus timing matters, such as video contention, floating bus behaviour, or interrupt pulse timing.

## Instruction Emulator

[`Z80InstructionEmulator`](API/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/index.md) executes one instruction per call. It still calls back for each memory and I/O action required by that instruction and returns the number of T-states consumed.

```csharp
var cpu = new Z80InstructionEmulator();
var memory = new byte[65536];

var tStates = cpu.ExecuteInstruction((action, address, data) =>
{
    switch (action)
    {
        case ActionRequired.OpcodeRead:
        case ActionRequired.MemoryRead:
            cpu.Data = memory[address];
            break;

        case ActionRequired.MemoryWrite:
            memory[address] = data;
            break;

        case ActionRequired.IORead:
            cpu.Data = ReadPort(address);
            break;

        case ActionRequired.IOWrite:
            WritePort(address, data);
            break;
    }
});
```

The instruction level emulator is faster than the step level emulator, so is best to use when speed of emulation is more important than exact timing.

## CPU State

Both emulators expose the same high-level state objects:

- [`Registers`](API/MrKWatkins.OakCpu.Z80/Z80Registers/index.md) provides the main and shadow Z80 registers.
- [`Flags`](API/MrKWatkins.OakCpu.Z80/Z80Flags/index.md) exposes individual flag bits.
- [`Interrupts`](API/MrKWatkins.OakCpu.Z80/Z80Interrupts/index.md) exposes interrupt flip-flops, interrupt mode, `HALT` state, and the external interrupt line.

Use `Reset()` to restore the CPU core to its reset state. Use `Serialize(Stream)`, `Restore(Stream)`, and `Deserialize(Stream)` to persist and reload CPU state.
