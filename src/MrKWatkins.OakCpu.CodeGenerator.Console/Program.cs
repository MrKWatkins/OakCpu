using MrKWatkins.OakCpu.CodeGenerator.Console;

await Generator.GenerateAsync("z80", "MrKWatkins.OakCpu.Z80").ConfigureAwait(false);
await Generator.GenerateAsync("z80n", "MrKWatkins.OakCpu.Z80N").ConfigureAwait(false);
await Generator.GenerateAsync("6502", "MrKWatkins.OakCpu.M6502").ConfigureAwait(false);