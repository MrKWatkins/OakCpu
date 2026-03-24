# AGENTS.md

This file provides guidance to AI agents when working with code in this repository.

## Project Overview

OakCpu is a cycle-accurate Z80 CPU emulator written in C# targeting .NET 10.0. CPU instructions are defined in YAML files (`definitions/z80/`) and a console app (`CodeGenerator.Console`) uses Roslyn to produce all emulator code — the Z80 project contains no hand-written instruction logic. The generated `.cs` files are checked into git.

## Build Commands

All commands run from the repo root.

```bash
dotnet build src/OakCpu.sln # Build all projects.
dotnet run --project src/MrKWatkins.OakCpu.CodeGenerator.Console --no-build # Regenerate checked-in .generated.cs files.
dotnet test --solution src/OakCpu.sln # Run the standard test suite.
dotnet test --project src/MrKWatkins.OakCpu.Z80.Tests --filter "FullyQualifiedName~InterruptsTests|FullyQualifiedName~TimingTests" # Focused interrupt/timing regression pass.
dotnet format src/OakCpu.sln # Format the source code.
```

For interrupt- and timing-related changes, do not stop at the broad solution test run. Also run the focused `InterruptsTests` and `TimingTests` filter above.

After regenerating with `CodeGenerator.Console`, do not use `dotnet test --no-build` for sign-off. The generator changes checked-in `.generated.cs` files, so the solution must be rebuilt as part of the post-regeneration validation pass.

## Architecture

### Code Generation Pipeline

1. **YAML definitions** (`definitions/z80/*.yaml`) define CPU instructions in a custom DSL — **never modify these files**
2. **CodeGenerator** (`MrKWatkins.OakCpu.CodeGenerator`) parses YAML via VYaml, processes through a custom lexer/parser/AST, and generates C# via Roslyn
3. **CodeGenerator.Console** (`MrKWatkins.OakCpu.CodeGenerator.Console`) is a console app that invokes CodeGenerator and writes `.generated.cs` files to disk
4. **Z80** (`MrKWatkins.OakCpu.Z80`) contains the emulator — all `.generated.cs` files are produced by CodeGenerator.Console and checked into git

Only ever manually change .generated.cs files when testing a change to the generated code. After working out the correct form for the .generated.cs files,
update the code generator to produce the new files, and run the code generation to validate.

### Key Projects

- **CodeGenerator** — Core code generation library (definitions parsing, AST, expression generation, optimizations)
- **CodeGenerator.Console** — Console app that runs CodeGenerator and writes generated files to disk
- **CodeGenerator.Tests** — Tests for the code generator
- **Z80** — The emulator itself (`Z80Emulator` partial class, all code generated)
- **Z80.Tests** — Emulator tests using FUSE, ZEXALL, and other standard test suites. Some tests are marked explicit (long-running)
- **Z80.Testing** — Test harness utilities (`Z80EmulatorTestHarness`)
- **Z80.Benchmarks** — BenchmarkDotNet performance tests — **never run in development**
- **Z80.Disassemble** — JIT assembly analysis tool — **never run in development**

## Code Style

- Always use `var` for local variables
- Always use braces for single-line statements
- Prefer `internal` over `public` where possible
- Prefer `readonly` fields and immutable types (e.g. return `IReadOnlyList<T>` not `List<T>`)
- Use collection expressions (e.g. `[]` instead of `Array.Empty<T>()`)
- Prefer lambdas over explicit method blocks
- End comments with a period
- Never add `<Reference>` elements — use `<ProjectReference>` or `<PackageReference>`
- Never leave unnecessary `using` statements
- Test naming: method name matching the tested method; suffix with parameter types for overloads, suffix with condition for edge cases (e.g. `MyMethod_ThrowsForNullArgument`)
- Do not write test classes for enums

## Build Configuration

- Target framework: `net10.0` with preview language features
- `TreatWarningsAsErrors=true` across all projects
- Central package management via `Directory.Packages.props`
- NUnit 4.x is the test framework
- Strict `.editorconfig` rules enforced
