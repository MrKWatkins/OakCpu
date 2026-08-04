using MrKWatkins.OakCpu.CodeGenerator.Console;

await Generator.GenerateAsync("z80", "MrKWatkins.OakCpu.Z80");
await Generator.GenerateAsync("z80n", "MrKWatkins.OakCpu.Z80N");
await Generator.GenerateAsync("6502", "MrKWatkins.OakCpu.M6502");