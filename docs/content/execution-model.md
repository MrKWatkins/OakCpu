# Execution Model

OakCpu keeps memory and I/O outside the CPU core. The emulator advances the Z80 state until it reaches a point where the host machine must perform an external action, then reports that action with [`ActionRequired`](api/MrKWatkins.OakCpu.Z80/ActionRequired/index.md).

This page describes the Z80. The 6502 follows the same execution model and action set; only the CPU-specific state, such as the interrupt model, differs.

## Actions

| Action | Host responsibility |
| ------ | ------------------- |
| `None` | No external bus action is required for this T-state. |
| `OpcodeRead` | Read the opcode byte at the supplied address and place it in `Data`. |
| `MemoryRead` | Read the byte at the supplied address and place it in `Data`. |
| `MemoryWrite` | Write `Data` to the supplied address. |
| `IORead` | Read from the supplied port address and place the result in `Data`. |
| `IOWrite` | Write `Data` to the supplied port address. |

The step emulator exposes the current bus address through [`Address`](api/MrKWatkins.OakCpu.Z80/Z80StepEmulator/Address.md) and the bus data latch through [`Data`](api/MrKWatkins.OakCpu.Z80/Z80StepEmulator/Data.md). The instruction emulator passes the address and current data value to the bus handler's `OnActionRequired` method and uses its [`Data`](api/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/Data.md) property for read results.

## Step-Level Execution

[`Z80StepEmulator.Step()`](api/MrKWatkins.OakCpu.Z80/Z80StepEmulator/Step.md) executes one T-state and returns immediately. This allows the host to advance other hardware on exactly the same timing boundary as the CPU.

Use [`IsAtInstructionBoundary`](api/MrKWatkins.OakCpu.Z80/Z80StepEmulator/IsAtInstructionBoundary.md) when a machine-level component needs to know whether the CPU is ready to start a new instruction.

## Pins

For CPUs that define pins, the step emulator exposes the bus control pins asserted during the most recently executed T-state through the `Pins` property; for the Z80 the values are the [`Z80Pins`](api/MrKWatkins.OakCpu.Z80/Z80Pins/index.md) flags. Active-low hardware pins are represented as active-high flags, sampled at the rising edge of the T-state's clock — the same view external synchronous logic has of the bus.

Together with `Address` and `Data` this gives hosts a hardware-shaped view of each machine cycle. An opcode fetch asserts `M1` at T1, `M1 | MREQ | RD` at T2, then `RFSH` and `RFSH | MREQ` during the refresh T-states, with the refresh address on the address bus. An interrupt acknowledge is identified by the `M1 | IORQ` combination, with the interrupted program counter on the address bus. The first T-state of memory and I/O cycles asserts no control pins — exactly as on hardware, where only the address bus distinguishes it — so host logic that decodes machine cycles from pins behaves like the real support chips.

`Pins` is `None` after `Reset` and undefined after a state restore until the next `Step`. The instruction emulator does not expose pins.

## Instruction-Level Execution

[`Z80InstructionEmulator.ExecuteInstruction`](api/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/ExecuteInstruction.md) executes until the current instruction, interrupt sequence, or `HALT` cycle completes. It is passed a bus handler implementing [`IZ80BusHandler`](api/MrKWatkins.OakCpu.Z80/IZ80BusHandler/index.md); the handler's `OnActionRequired` method receives each external action in order, so memory and I/O behaviour remains host-controlled. Passing the handler as a generic `struct` lets the JIT inline the bus access into the execution loop. See [Using the Emulator](using-the-emulator.md#instruction-emulator) for an example.

## Interrupts

The [`Interrupts`](api/MrKWatkins.OakCpu.Z80/Z80Interrupts/index.md) object exposes the interrupt state:

- `IFF1` and `IFF2` are the Z80 interrupt flip-flops.
- `IM` is the interrupt mode.
- `Interrupt` represents the external interrupt line and is controlled by the host.
- `Halted` indicates whether the CPU is in `HALT`.

Set `Interrupt` to `true` while the external interrupt line is asserted, and clear it when the host hardware deasserts the line.

## State Persistence

Both emulators can serialise their CPU state to a [`Stream`](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream). Serialisation covers CPU registers, flags, interrupt state, pending execution state, and internal latches. It does not serialise host memory, I/O devices, or other machine components.
