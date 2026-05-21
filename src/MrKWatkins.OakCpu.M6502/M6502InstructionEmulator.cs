namespace MrKWatkins.OakCpu.M6502;

public sealed partial class M6502InstructionEmulator
{
    internal void SetIRQ(bool value)
    {
        irq = value;
        sampledirq = value;
    }
    internal void SetNMI(bool value)
    {
        nmi = value;
        if (value && !previousnmi)
        {
            pendingnmi = true;
        }

        previousnmi = value;
    }
}