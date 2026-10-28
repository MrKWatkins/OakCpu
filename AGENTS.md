# AGENTS.md

This file provides guidance to AI agents when working with code in this repository. It exists in three places via symlinks, AGENTS.md, CLAUDE.md and .github/copilot-instructions.md.

## Project Overview

OakCpu is a C# library providing cycle-accurate emulators for 8-bit CPUs. The Z80 is the mature implementation, published as the `MrKWatkins.OakCpu.Z80` NuGet package; a 6502 implementation also exists as `MrKWatkins.OakCpu.M6502`.

Implementations are generated from YAML instruction definitions in `definitions/<cpu>`, e.g. `definitions/z80`. The generated `.cs` files are checked into git, so normal consumers and builds do not need to run the generator.

## Build & Test Commands

All commands run from the root directory:

```bash
dotnet build src/OakCpu.slnx # Build all projects.

(cd src && dotnet run --project MrKWatkins.OakCpu.CodeGenerator.Console --no-build) # Regenerate checked-in .generated.cs files for all CPUs. The generator finds OakCpu.slnx by searching upwards, so it must run from src.
(cd src && dotnet run --project MrKWatkins.OakCpu.DocumentationGenerator.Console --no-build) # Regenerate the checked-in Markdown instruction documentation in docs/instructions for the Z80 and Z80N. Also run from src.

./src/MrKWatkins.OakCpu.Definitions.Tests/bin/Debug/net10.0/MrKWatkins.OakCpu.Definitions.Tests # Run the definitions model tests.
./src/MrKWatkins.OakCpu.CodeGenerator.Tests/bin/Debug/net10.0/MrKWatkins.OakCpu.CodeGenerator.Tests # Run the code generator tests.
./src/MrKWatkins.OakCpu.DocumentationGenerator.Tests/bin/Debug/net10.0/MrKWatkins.OakCpu.DocumentationGenerator.Tests # Run the documentation generator tests.
./src/MrKWatkins.OakCpu.Z80.Tests/bin/Debug/net10.0/MrKWatkins.OakCpu.Z80.Tests # Run the Z80 emulator tests.
./src/MrKWatkins.OakCpu.M6502.Tests/bin/Debug/net10.0/MrKWatkins.OakCpu.M6502.Tests # Run the 6502 emulator tests.

dotnet format src/OakCpu.slnx # Format the source code.
```

The test projects use NUnit on Microsoft Testing Platform (`EnableNUnitRunner`). `src/global.json` selects native MTP mode, so run tests from `src`:

```bash
(cd src && dotnet test --solution OakCpu.slnx) # Run all non-explicit tests.
```

The test executables above can also run directly. The shared build uses native MTP to collect coverage. Keep the explicitly marked long-running emulator suites excluded from normal builds.

A plain run of a test executable runs every non-`[Explicit]` test, which includes the interrupt regression tests (`InterruptsTests` and `InterruptBoundaryTests`). The large skipped count in the Z80 run is the `[Explicit]` suites — SingleStep, ZEXALL, Raxoft and Mark Woodmass.

After regenerating with `CodeGenerator.Console`, always rebuild before running the tests for sign-off. The generator changes checked-in `.generated.cs` files, so a stale build will test the old emulator.

## Architecture

The repository is organised around a code generation pipeline and the generated emulators:

- **YAML definitions** (`definitions/<cpu>/*.yaml`, e.g. `definitions/z80`) define CPU registers, flags, interrupts, and instructions. Do not change these files unless the task is specifically instruction-definition or documentation work. Each instruction has a `documentation` description for the generated documentation, with operand placeholders such as `{R0}` in braces; instructions, opcodes and registers can be marked `undocumented: true`; and an instruction can describe a flag by hand with `flag_documentation` when its expression is too involved to describe automatically.
- **`MrKWatkins.OakCpu.Definitions`** parses YAML via VYaml, processes the instruction expression DSL through a custom lexer/parser/AST, validates the definitions and builds the `CpuModel` the generators work from. It has no dependency on Roslyn.
- **`MrKWatkins.OakCpu.CodeGenerator`** generates C# with Roslyn from the model, adding the Roslyn syntax the model needs as extension members.
- **`MrKWatkins.OakCpu.CodeGenerator.Console`** invokes the generator and writes checked-in `.generated.cs` files to disk. The CPUs to generate, and the project each is written to, are a hardcoded list in its `Program.cs`.
- **`MrKWatkins.OakCpu.DocumentationGenerator`** generates Markdown documentation for every instruction from the model: a page per instruction form, e.g. `LD R0, (RP0 + d)`, with YAML front matter carrying the description, the effect on each flag derived from the instruction's flag expressions, and every opcode with its encoding, operands, T-states and whether it is undocumented. T-states are calculated from the steps, and the undocumented status of duplicate opcodes from the definitions.
- **`MrKWatkins.OakCpu.DocumentationGenerator.Console`** writes the checked-in documentation to `docs/instructions/<cpu>`. The CPUs to document are a hardcoded list in its `Program.cs`.
- **`MrKWatkins.OakCpu.SourceGenerator`** uses the same generator library as a Roslyn source generator. It is not in the solution, is not referenced by anything, and does not currently compile against the generator API; the console generator is the live path.
- **`MrKWatkins.OakCpu.Z80`** and **`MrKWatkins.OakCpu.M6502`** contain the emulator packages. Nearly all the code is generated; the only handwritten files are the partial-class runtime entry points, `Z80StepEmulator.cs` for the Z80 and `M6502StepEmulator.cs`/`M6502InstructionEmulator.cs` for the 6502.
- **`MrKWatkins.OakCpu.Z80.Testing`** and **`MrKWatkins.OakCpu.M6502.Testing`** contain emulator test harness utilities, built on the `MrKWatkins.EmulatorTestSuites` packages.
- **`MrKWatkins.OakCpu.Definitions.Tests`** tests the YAML schema, parser, expression model, validation and model.
- **`MrKWatkins.OakCpu.CodeGenerator.Tests`** tests the generator and generated output. Generator output is covered by Verify.NUnit snapshots (`{Class}.{Method}.verified.txt` beside the test source).
- **`MrKWatkins.OakCpu.DocumentationGenerator.Tests`** tests the documentation generator, including checking the calculated T-states against the `MrKWatkins.OakAsm` packages and Verify.NUnit snapshots of the Markdown.
- **`MrKWatkins.OakCpu.Z80.Tests`** and **`MrKWatkins.OakCpu.M6502.Tests`** test the emulators. The test data comes from the `MrKWatkins.EmulatorTestSuites` NuGet packages, not from this repository.
- **`MrKWatkins.OakCpu.Z80.Benchmarks`** contains BenchmarkDotNet performance tests. Do not run benchmarks during normal development.

Only manually change `.generated.cs` files when experimenting with the desired generated output. Once the correct output is known, update the generator and regenerate the files.

### Variant CPUs

A CPU can be defined as a variant of another CPU by setting `base:` in the `cpu:` block of its `cpu.yaml`, naming the definitions directory of the CPU it is based on, e.g. `base: z80`. The variant then contains only its additions and overrides; everything else is inherited, so base definitions are never duplicated.

The merge is performed by `YamlFile.Merge`, before validation:

- Pins, actions, fields, registers and flags from the variant are **appended** to those of the base CPU. Do not reorder them: `ActionRequired` and pin values are assigned by list position, so appending keeps the values generated for the base CPU stable.
- Sequences and functions **replace** a base one with the same name.
- The CPU name, and any hooks (`on_instruction_complete` etc.), interrupts and `default_next_opcode` the variant defines, take precedence; anything it omits is inherited.
- Instructions override **individual opcodes**, not whole instructions. If a variant instruction claims an opcode belonging to a base instruction, only that opcode is removed from the base instruction, which is dropped entirely only if it loses all of its opcodes. This matters because base instructions commonly cover many opcodes at once, e.g. the Z80 `NOP` covers all the unused `ED` opcodes.

## Code Conventions

- Always use `var` for local variables.
- Always use braces for single-line statements.
- Prefer `internal` over `public` where possible.
- Prefer `readonly` fields and immutable return types, e.g. `IReadOnlyList<T>` instead of `List<T>` or arrays.
- Use collection expressions, e.g. `[]` instead of `Array.Empty<T>()`.
- Prefer lambdas over explicit method blocks where possible.
- Put nested types at the end of their parent type rather than the start.
- Mark side-effect-free methods with `[Pure]`. If a method mutates passed-in state but the caller should still use the returned value, use `[MustUseReturnValue]` instead of `[Pure]`.
- Use `6502` for the definitions folder and for human-facing comments/documentation; keep `M6502` only where a C# identifier requires it, such as project names, namespaces, and types.
- End comments with a period.
- Never leave unnecessary `using` statements.
- Never add `<Reference>` elements to project files; use `<ProjectReference>` or `<PackageReference>`.
- Global usings are configured in `src/Directory.Build.props`: `System.Diagnostics.CodeAnalysis`, `System.Diagnostics.Contracts`, `PureAttribute`, and `JetBrains.Annotations`.
- Target framework is `net10.0`; warnings are errors; packages are centrally managed in `src/Directory.Packages.props`.

## Testing Conventions

- NUnit 4 with `MrKWatkins.Assertions`. (`.Should().Equal()`, `.Should().Throw<>()`, `AssertThat.Invoking()`)
- Global usings for `MrKWatkins.Assertions` and `NUnit.Framework` are configured for test projects.
- Test names should match the method they're testing.
- If a method is overloaded then test names should distinguish overloads by their parameter types, separated by underscores.
- Extra conditions can be appended to the end of test names to distinguish them, e.g. `MyMethod_ThrowsForNullArgument`. The happy path should not have extra conditions.
- Do not write test classes for enums.
- InternalsVisibleTo is automatically configured for test projects.
- Some Z80 test suites include explicit long-running tests; do not run explicit tests unless the task requires them.

## Formatting Conventions

- Ensure formatting is correct by running `dotnet format src/OakCpu.slnx` after completing code changes.
- Do not leave generated build, coverage, documentation site, or temporary files in the working tree.

## Documentation

- Handwritten Markdown lives in `docs/content`; guide navigation and all three API assembly inputs (6502, Z80 and Z80N) are configured in `docs/site.json`.
- Sesharp, Vale and VitePress run in the shared documentation image. Generated APIs and site output stay in ignored `artifacts`; never commit them.
- Preview with `python3 docs/preview.py` or the JetBrains **Preview documentation** run configuration, then open `http://localhost:5173/OakCpu/`. Docker and the .NET 10 SDK are required.
- Build and Release call `MrKWatkins/build/dotnet/project`. Release can publish packages, documentation, or both, using NuGet trusted publishing.
- Handwritten documentation should link to generated APIs under `api/` and Microsoft's API docs (https://learn.microsoft.com/en-us/dotnet/api/) for framework types and members.
- `ReadMe.md` gives a brief summary of the project, links to the full documentation, and is included in the NuGet package.
