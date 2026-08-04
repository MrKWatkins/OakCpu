# Code Generation

OakCpu's Z80 and 6502 implementations are generated from YAML definitions in the repository's `definitions` directory — `definitions/z80` and `definitions/6502`. The generated C# is checked into source control so the emulator packages can be built without running the generator during normal package consumption.

## Pipeline

1. YAML files define registers, flags, interrupt handling, and instructions.
2. `MrKWatkins.OakCpu.CodeGenerator` parses the YAML and the instruction expression DSL.
3. The generator emits C# source for the step and instruction emulators of each CPU.
4. `MrKWatkins.OakCpu.CodeGenerator.Console` regenerates the checked-in `.generated.cs` files for every CPU.
5. `MrKWatkins.OakCpu.SourceGenerator` uses the same generator library for Roslyn source generation.

## Regenerating Source

Run the console generator from the `src` directory. It locates the solution by searching upwards for `OakCpu.sln`, which lives there, so it must be run from `src` or a subdirectory of it:

```bash
cd src
dotnet run --project MrKWatkins.OakCpu.CodeGenerator.Console --no-build
```

This regenerates the checked-in files for every CPU. After regenerating, rebuild the solution before testing. The generated files are part of the committed source tree, so a fresh build is needed to validate the emitted code.

## Editing Generated Code

Generated `.cs` files should not be edited directly for permanent changes. If a generated file needs to change, update the generator or the handwritten runtime code that feeds it, then run the generator again.

The YAML instruction definitions are the source of truth for CPU behaviour and should only be changed deliberately as part of instruction definition work.

## Variant CPUs

A CPU can be defined as a variant of another by setting `base` in the `cpu` block of its `cpu.yaml`, naming the definitions directory of the CPU it is based on. The Z80N, the Z80 variant in the ZX Spectrum Next, is defined this way:

```yaml
cpu:
  name: Z80N
  base: z80
```

The variant contains only its additions and overrides; everything else is inherited, so the definitions it is based on are never duplicated. The merge happens before validation, and:

* Pins, actions, fields, registers and flags in the variant are appended to those of the base CPU, so the values generated for the base CPU's members do not change.
* Sequences and functions replace a base one of the same name.
* Instructions override individual opcodes rather than whole instructions. A base instruction that covers several opcodes, such as the Z80's `NOP` covering the unused `ED` prefixed opcodes, keeps the opcodes the variant does not claim, and only disappears if it loses all of them.
