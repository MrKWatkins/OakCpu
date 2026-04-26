# OakCpu - CPU Emulators

OakCpu is a .NET 10.0 C# emulator for CPUs. Only the Z80 is currently emulated. The Z80 emulator is a cycle-accurate (as opposed to an instruction level accurate)
emulator. The instructions for the emulator are defined in YAML files in the definitions directory. A Roslyn source generator is used to generate the emulator code from
the YAML files.

Always reference these instructions first and fall back to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Initial Setup and Dependencies
- Install .NET 9.0 SDK: `wget https://dot.net/v1/dotnet-install.sh && chmod +x dotnet-install.sh && ./dotnet-install.sh --version 9.0.103`
- Add to PATH: `export PATH="/home/runner/.dotnet:$PATH"`
- Clean up install script: `rm dotnet-install.sh` (optional)
- Restore dependencies: `dotnet restore` -- **NEVER CANCEL: Takes 98 seconds on first run (2 seconds on subsequent runs due to caching). Set timeout to 180+ seconds.**
- Build solution: `dotnet build --no-restore --configuration Release` -- **NEVER CANCEL: Takes 52 seconds on first run (30 seconds on subsequent runs due to caching). Set timeout to 120+ seconds.**

### Testing
- Run all tests with code coverage: `dotnet test --no-restore --no-build --configuration Release --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Testing]*` -- **NEVER CANCEL. Set timeout to 120+ seconds.**
- Run a specific test project by substituting <project> with the full path to the .csproj file in: `dotnet test <project> --no-restore --no-build --configuration Release --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Testing]*` -- **NEVER CANCEL. Set timeout to 120+ seconds.**

### Code Quality
- **ALWAYS** run `dotnet format --verify-no-changes` before committing changes.
- The project has strict code analysis rules configured in .editorconfig
- All projects use `TreatWarningsAsErrors=true`
- **NEVER** leave unnecessary `using` statements in the code.

## Validation

### Always Test After Changes
- Build the solution to ensure no compilation errors
- Always run tests to verify functionality
- Verify code formatting with `dotnet format --verify-no-changes`

### CI Validation
- The GitHub Actions workflow (`.github/workflows/build.yml`) runs:
  1. Dependency restoration
  2. Code formatting verification
  3. Release build
  4. Full test suite
  5. Coverage report generation
- **Always ensure your changes pass formatting**: `dotnet format --verify-no-changes`

## Project Structure

- **MrKWatkins.OakCpu.CodeGenerator**: Core Roslyn code generation library
- **MrKWatkins.OakCpu.CodeGenerator.Tests**: Unit tests for **MrKWatkins.OakCpu.CodeGenerator**. Make sure these tests pass after any changes to the generator.
- **MrKWatkins.OakCpu.SourceGenerator**: The actual source generator that uses the **MrKWatkins.OakCpu.CodeGenerator** library
- **MrKWatkins.OakCpu.Cpus.Z80**: Z80 CPU implementation. Contains no source files; all the code is generated with **MrKWatkins.OakCpu.SourceGenerator**
- **MrKWatkins.OakCpu.Cpus.Z80.Benchmarks**: Various benchmarks for the Z80 CPU. **NEVER RUN** benchmarks in regular development
- **MrKWatkins.OakCpu.Cpus.Z80.Disassemble**: Test program to help analyse the JIT x86 assembly. **NEVER RUN** in regular development
- **MrKWatkins.OakCpu.Cpus.Z80.Tests**: Unit tests for **MrKWatkins.OakCpu.Cpus.Z80**. These use the **MrKWatkins.EmulatorTestSuites.Z80** package for the tests.
  Make sure these tests pass after any changes to the generator. Some tests are marked explicit due to taking a long time to run.
- **definitions**: YAML files defining the Z80 CPU instructions. **NEVER CHANGE THESE FILES**

## Build System Details

### Dependencies Management
- Uses central package management via `Directory.Packages.props`
- Key dependencies:
  - NUnit (testing framework)
  - BenchmarkDotNet (performance testing)

### Target Framework
- All projects target .NET 9.0 (`net9.0`)
- Uses C# Preview language features (`LangVersion>Preview`)
- Requires .NET 9.0 SDK to build

### Package Publishing
- Core libraries are packaged as NuGet packages
- Release workflow publishes to NuGet on manual trigger

## Important Notes

### Code Style
- Strict adherence to configured EditorConfig rules
- Comprehensive code analysis rules with warnings treated as errors
- Use `dotnet format` to fix formatting issues automatically
- Prefer `internal` access modifiers over `public` where possible
- Prefer `readonly` fields over mutable ones
- Prefer immutable types where possible, e.g. return `IReadOnlyList<T>` instead of `List<T>` or `T[]`
- Use FluentAssertions for assertions in tests
- Use collection expressions where possible, e.g. instead of Array.Empty<T>()
- Always use `var` for local variable declarations always
- Always use braces to enclose single-line statements
- Test method should have the same name as the method being tested. If there are overloads, suffix with the overload parameter types, separated by underscores. Normal positive tests can just be the name. Tests for other conditions, e.g. exceptions, should be suffixed with details, e.g. `MyMethod_ThrowsForNullArgument`
- Never add `<Reference>` elements to .csproj files. Always use `<ProjectReference>` or `<PackageReference>`
- End comments with a period, e.g. `// This is a comment.`
- Do not write test classes for enums as they do not add value
- Prefer lambdas over explicit method blocks where possible, e.g. `() => value` instead of `() => { return value; }`
- Do not leave unnecessary `using` statements in the code.