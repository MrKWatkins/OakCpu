# Code Generation

OakCpu's Z80 and 6502 implementations are generated from YAML definitions in the repository's `definitions` directory — `definitions/z80` and `definitions/6502`. The generated C# is checked into source control so the emulator packages can be built without running the generator during normal package consumption.

## Pipeline

1. YAML files define registers, flags, interrupt handling, and instructions.
2. `MrKWatkins.OakCpu.Definitions` parses the YAML and the instruction expression DSL into a model of the CPU.
3. `MrKWatkins.OakCpu.CodeGenerator` emits C# source for the step and instruction emulators of each CPU from the model.
4. `MrKWatkins.OakCpu.CodeGenerator.Console` regenerates the checked-in `.generated.cs` files for every CPU.
5. `MrKWatkins.OakCpu.SourceGenerator` uses the same generator library for Roslyn source generation.
6. `MrKWatkins.OakCpu.DocumentationGenerator` generates Markdown documentation for every instruction from the same model, and `MrKWatkins.OakCpu.DocumentationGenerator.Console` regenerates the checked-in documentation in `docs/instructions`.

## Regenerating Source

Run the console generator from the `src` directory. It locates the solution by searching upwards for `OakCpu.slnx`, which lives there, so it must be run from `src` or a subdirectory of it:

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

## Instruction Documentation

The definitions also document the instructions. Each instruction has a `documentation` description, with the placeholders from its mnemonic in braces, for example `Loads the byte at ({RP0} + d) into {R0}.` for `LD R0, (RP0 + d)`. Instructions, opcodes and registers that are not in the manufacturer's documentation are marked `undocumented: true`; prefixed copies of other opcodes are worked out from the definitions and need no marking. The rest comes from the definitions themselves: the effect on each flag is described from the instruction's flag expressions, and the T-states are calculated from its steps, with a `flag_documentation` entry describing a flag by hand where its expression is too involved to describe automatically.

The documentation generator writes a Markdown page per instruction form to `docs/instructions/<cpu>`, with YAML front matter carrying everything on the page as data, so that other tools can use it without parsing the Markdown. Run it from the `src` directory like the code generator:

```bash
cd src
dotnet run --project MrKWatkins.OakCpu.DocumentationGenerator.Console --no-build
```
