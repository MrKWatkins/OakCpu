using MrKWatkins.OakCpu.Z80;

var emulator = new Z80StepEmulator();
for (var f = 0; f < 5; f++)
{
    emulator.Step();
    emulator.Data = 0x3C;
}