# AGENTS.md

This file provides guidance to AI agents when working with code in this repository.

## Project Overview

OakCpu is a C# library providing cycle-accurate emulators for 8-bit CPUs. The only CPU currently implemented is the Z80, published as the `MrKWatkins.OakCpu.Z80` NuGet package.

The Z80 implementation is generated from YAML instruction definitions in `definitions/z80`. The generated `.cs` files are checked into git, so normal consumers and builds do not need to run the generator.

## Build & Test Commands

All commands run from the root directory:

```bash
dotnet build src/OakCpu.sln # Build all projects.

(cd src && dotnet run --project MrKWatkins.OakCpu.CodeGenerator.Console --no-build) # Regenerate checked-in .generated.cs files for all CPUs. The generator finds OakCpu.sln by searching upwards, so it must run from src.

dotnet test --solution src/OakCpu.sln # Run all tests.
dotnet test --project src/MrKWatkins.OakCpu.CodeGenerator.Tests # Run the code generator tests.
dotnet test --project src/MrKWatkins.OakCpu.Z80.Tests # Run the Z80 emulator tests.
dotnet test --project src/MrKWatkins.OakCpu.Z80.Tests --filter "FullyQualifiedName~InterruptsTests|FullyQualifiedName~TimingTests" # Run interrupt and timing regression tests.
dotnet test --project src/MrKWatkins.OakCpu.Z80.Tests --filter "FullyQualifiedName~Z80StepEmulatorTests" # Run a single test class.

dotnet format src/OakCpu.sln # Format the source code.
```

For interrupt- and timing-related changes, do not stop at the broad solution test run. Also run the focused `InterruptsTests` and `TimingTests` filter above.

After regenerating with `CodeGenerator.Console`, do not use `dotnet test --no-build` for sign-off. The generator changes checked-in `.generated.cs` files, so the solution must be rebuilt as part of the post-regeneration validation pass.

The test projects use the NUnit runner with Microsoft Testing Platform (`EnableNUnitRunner`), so test executables can also be run directly.

## Architecture

The repository is organised around a code generation pipeline and the generated Z80 emulator:

- **YAML definitions** (`definitions/z80/*.yaml`) define CPU registers, flags, interrupts, and instructions. Do not change these files unless the task is specifically instruction-definition work.
- **`MrKWatkins.OakCpu.CodeGenerator`** parses YAML via VYaml, processes the instruction expression DSL through a custom lexer/parser/AST, and generates C# with Roslyn.
- **`MrKWatkins.OakCpu.CodeGenerator.Console`** invokes the generator and writes checked-in `.generated.cs` files to disk.
- **`MrKWatkins.OakCpu.SourceGenerator`** uses the same generator library as a Roslyn source generator.
- **`MrKWatkins.OakCpu.Z80`** contains the emulator package. Most code is generated; handwritten files provide runtime entry points such as `Z80StepEmulator` and `Z80InstructionEmulator`.
- **`MrKWatkins.OakCpu.Z80.Testing`** contains emulator test harness utilities.
- **`MrKWatkins.OakCpu.CodeGenerator.Tests`** tests the generator, parser, expression model, and generated output.
- **`MrKWatkins.OakCpu.Z80.Tests`** tests the emulator with FUSE, ZEXALL, timing, interrupt, and other Z80 test suites.
- **`MrKWatkins.OakCpu.Z80.Benchmarks`** contains BenchmarkDotNet performance tests. Do not run benchmarks during normal development.
- **`MrKWatkins.OakCpu.Z80.Disassemble`** is a JIT assembly analysis tool. Do not run it during normal development.

Only manually change `.generated.cs` files when experimenting with the desired generated output. Once the correct output is known, update the generator and regenerate the files.

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

- Ensure formatting is correct by running `dotnet format src/OakCpu.sln` after completing code changes.
- Do not leave generated build, coverage, documentation site, or temporary files in the working tree.

## Documentation

- Documentation is generated using MKDocs and is found in the `doc` folder.
- Documentation in `doc/docs/API` is generated from the Z80 and 6502 assemblies using the Sesharp tool from the root of the repository: `sesharp src/MrKWatkins.OakCpu.M6502/bin/Release/net10.0/MrKWatkins.OakCpu.M6502.dll src/MrKWatkins.OakCpu.Z80/bin/Release/net10.0/MrKWatkins.OakCpu.Z80.dll --output doc/docs/API --repository https://github.com/MrKWatkins/OakCpu`
- Documentation in the root of `doc/docs` is handwritten.
- Handwritten documentation should link to the generated API documentation and Microsoft's API docs (https://learn.microsoft.com/en-us/dotnet/api/) for types, members, etc.
- `ReadMe.md` gives a brief summary of the project, links to the full documentation, and is included in the NuGet package.
