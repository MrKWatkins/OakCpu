# Code Generation

OakCpu's Z80 implementation is generated from YAML definitions in the repository's `definitions/z80` directory. The generated C# is checked into source control so the emulator package can be built without running the generator during normal package consumption.

## Pipeline

1. YAML files define registers, flags, interrupt handling, and instructions.
2. `MrKWatkins.OakCpu.CodeGenerator` parses the YAML and the instruction expression DSL.
3. The generator emits C# source for the step and instruction emulators.
4. `MrKWatkins.OakCpu.CodeGenerator.Console` regenerates the checked-in `.generated.cs` files.
5. `MrKWatkins.OakCpu.SourceGenerator` uses the same generator library for Roslyn source generation.

## Regenerating Source

Run the console generator from the repository root:

```bash
dotnet run --project src/MrKWatkins.OakCpu.CodeGenerator.Console --no-build
```

After regenerating, rebuild the solution before testing. The generated files are part of the committed source tree, so a fresh build is needed to validate the emitted code.

## Editing Generated Code

Generated `.cs` files should not be edited directly for permanent changes. If a generated file needs to change, update the generator or the handwritten runtime code that feeds it, then run the generator again.

The YAML instruction definitions are the source of truth for CPU behaviour and should only be changed deliberately as part of instruction definition work.
