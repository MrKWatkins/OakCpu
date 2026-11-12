using MrKWatkins.OakCpu.DocumentationGenerator.Console;

await Generator.GenerateAsync("z80").ConfigureAwait(false);
await Generator.GenerateAsync("z80n").ConfigureAwait(false);
await Generator.GenerateAsync("6502").ConfigureAwait(false);