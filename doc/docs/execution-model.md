# Execution Model

OakCpu keeps memory and I/O outside the CPU core. The emulator advances the Z80 state until it reaches a point where the host machine must perform an external action, then reports that action with [`ActionRequired`](API/MrKWatkins.OakCpu.Z80/ActionRequired/index.md).

## Actions

| Action | Host responsibility |
| ------ | ------------------- |
| `None` | No external bus action is required for this T-state. |
| `OpcodeRead` | Read the opcode byte at the supplied address and place it in `Data`. |
| `MemoryRead` | Read the byte at the supplied address and place it in `Data`. |
| `MemoryWrite` | Write `Data` to the supplied address. |
| `IoRead` | Read from the supplied port address and place the result in `Data`. |
| `IoWrite` | Write `Data` to the supplied port address. |

The step emulator exposes the current bus address through [`Address`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/Address.md) and the bus data latch through [`Data`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/Data.md). The instruction emulator passes the address and current data value to its callback and uses its [`Data`](API/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/Data.md) property for read results.

## Step-Level Execution

[`Z80StepEmulator.Step()`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/Step.md) executes one T-state and returns immediately. This allows the host to advance other hardware on exactly the same timing boundary as the CPU.

Use [`IsAtInstructionBoundary`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/IsAtInstructionBoundary.md) when a machine-level component needs to know whether the CPU is ready to start a new instruction.

## Instruction-Level Execution

[`Z80InstructionEmulator.ExecuteInstruction`](API/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/ExecuteInstruction.md) executes until the current instruction, interrupt sequence, or `HALT` cycle completes. The callback receives each external action in order, so memory and I/O behaviour remains host-controlled.

## Interrupts

The [`Interrupts`](API/MrKWatkins.OakCpu.Z80/Z80Interrupts/index.md) object exposes the interrupt state:

- `IFF1` and `IFF2` are the Z80 interrupt flip-flops.
- `IM` is the interrupt mode.
- `Interrupt` represents the external interrupt line and is controlled by the host.
- `Halted` indicates whether the CPU is in `HALT`.

Set `Interrupt` to `true` while the external interrupt line is asserted, and clear it when the host hardware deasserts the line.

## State Persistence

Both emulators can serialise their CPU state to a [`Stream`](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream). Serialisation covers CPU registers, flags, interrupt state, pending execution state, and internal latches. It does not serialise host memory, I/O devices, or other machine components.
