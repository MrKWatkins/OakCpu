# Home

[![Build Status](https://github.com/MrKWatkins/OakCpu/actions/workflows/build.yml/badge.svg)](https://github.com/MrKWatkins/OakCpu/actions/workflows/build.yml)
[![NuGet Version](https://img.shields.io/nuget/v/MrKWatkins.OakCpu.Z80)](https://www.nuget.org/packages/MrKWatkins.OakCpu.Z80)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MrKWatkins.OakCpu.Z80)](https://www.nuget.org/packages/MrKWatkins.OakCpu.Z80)

> Cycle-accurate emulators for 8-bit CPUs.

OakCpu is a collection of .NET emulators for CPUs. Currently, only the Z80 is supported. Two varints are available, a step level emulator that advances a single T-state at a time, and an instruction level emulator that advances a single instruction at a time.

## Installation

```bash
dotnet add package MrKWatkins.OakCpu.Z80
```

## Using the Emulator

Create either a step emulator for cycle-accurate integration or an instruction emulator when you only need instruction-level callbacks.

[Read more](using-the-emulator.md)

## Execution Model

The emulator reports external bus activity with [`ActionRequired`](API/MrKWatkins.OakCpu.Z80/ActionRequired/index.md). The host supplies or consumes data through the emulator's `Address` and `Data` properties.

[Read more](execution-model.md)

## Code Generation

The Z80 implementation is generated from YAML instruction definitions. Generated source is checked in, and the same generator is used by the source generator package.

[Read more](code-generation.md)

## API Documentation

Reference documentation is generated from the release assemblies:

- [`Z80StepEmulator`](API/MrKWatkins.OakCpu.Z80/Z80StepEmulator/index.md)
- [`Z80InstructionEmulator`](API/MrKWatkins.OakCpu.Z80/Z80InstructionEmulator/index.md)
- [`Z80Registers`](API/MrKWatkins.OakCpu.Z80/Z80Registers/index.md)
- [`Z80Flags`](API/MrKWatkins.OakCpu.Z80/Z80Flags/index.md)
- [`Z80Interrupts`](API/MrKWatkins.OakCpu.Z80/Z80Interrupts/index.md)

## Future Plans

More CPUs will be supported. I intend to create emulators for other languages too, using the shared YAML definitions as the source of truth for emulator behaviour.

## Licencing

Licensed under GPL v3.0.
