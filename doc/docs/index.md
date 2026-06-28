# Home

[![Build Status](https://github.com/MrKWatkins/OakCpu/actions/workflows/build.yml/badge.svg)](https://github.com/MrKWatkins/OakCpu/actions/workflows/build.yml)
[![Z80 NuGet Version](https://img.shields.io/nuget/v/MrKWatkins.OakCpu.Z80?label=Z80%20Nuget%20Version)](https://www.nuget.org/packages/MrKWatkins.OakCpu.Z80)
[![Z80 NuGet Downloads](https://img.shields.io/nuget/dt/MrKWatkins.OakCpu.Z80?label=Z80%20Nuget%20Downloads)](https://www.nuget.org/packages/MrKWatkins.OakCpu.Z80)
[![6502 NuGet Version](https://img.shields.io/nuget/v/MrKWatkins.OakCpu.M6502?label=6502%20Nuget%20Version)](https://www.nuget.org/packages/MrKWatkins.OakCpu.M6502)
[![6502 NuGet Downloads](https://img.shields.io/nuget/dt/MrKWatkins.OakCpu.M6502?label=6502%20Nuget%20Downloads)](https://www.nuget.org/packages/MrKWatkins.OakCpu.M6502)

> Cycle-accurate emulators for 8-bit CPUs.

OakCpu is a collection of .NET emulators for CPUs. The Z80 and 6502 are currently supported. Each comes in two variants: a step level emulator that advances a single T-state at a time, and an instruction level emulator that advances a single instruction at a time.

## Installation

```bash
dotnet add package MrKWatkins.OakCpu.Z80
```

The 6502 is available in a separate package:

```bash
dotnet add package MrKWatkins.OakCpu.M6502
```

## Using the Emulator

Create either a step emulator for cycle-accurate integration or an instruction emulator when you only need to handle each external bus action one instruction at a time.

[Read more](using-the-emulator.md)

## Execution Model

The emulator reports external bus activity with [`ActionRequired`](API/MrKWatkins.OakCpu.Z80/ActionRequired/index.md). The host supplies or consumes data through the emulator's `Address` and `Data` properties.

[Read more](execution-model.md)

## Code Generation

The Z80 and 6502 implementations are generated from YAML instruction definitions. Generated source is checked in, and the same generator is used by the source generator package.

[Read more](code-generation.md)

## API Documentation

Reference documentation is generated from the release assemblies:

Z80:

- [`Z80StepEmulator`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/index.md)
- [`Z80InstructionEmulator`](API/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/index.md)
- [`Z80Registers`](API/MrKWatkins.OakCpu.Z80/Z80Registers/index.md)
- [`Z80Flags`](API/MrKWatkins.OakCpu.Z80/Z80Flags/index.md)
- [`Z80Interrupts`](API/MrKWatkins.OakCpu.Z80/Z80Interrupts/index.md)

6502:

- [`M6502StepEmulator`](API/MrKWatkins.OakCpu.M6502/M6502StepEmulator/index.md)
- [`M6502InstructionEmulator`](API/MrKWatkins.OakCpu.M6502/M6502InstructionEmulator/index.md)
- [`M6502Registers`](API/MrKWatkins.OakCpu.M6502/M6502Registers/index.md)
- [`M6502Flags`](API/MrKWatkins.OakCpu.M6502/M6502Flags/index.md)
- [`M6502Interrupts`](API/MrKWatkins.OakCpu.M6502/M6502Interrupts/index.md)

## Future Plans

More CPUs will be supported. I intend to create emulators for other languages too, using the shared YAML definitions as the source of truth for emulator behaviour.

## Use of AI

My general rule is I'll write the interesting bits and use AI for the boring bits. The step level Z80 emulator and instructions was created by hand. AI was then used to help with tidying up the code, performance improvements, adding the instruction level emulator and the documentation. The 6502 emulator was done entirely by AI, mainly to test the validity of adding a new set of definitions for another chip.

## Licencing

Licensed under GPL v3.0.
