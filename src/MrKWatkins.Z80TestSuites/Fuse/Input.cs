namespace MrKWatkins.Z80TestSuites.Fuse;

public sealed class Input : Z80State
{
    private Input()
    {
    }

    [Pure]
    internal static Input Parse(StreamReader reader)
    {
        var state = new Input();
        Parse(reader.ReadLine()!, reader, state);
        return state;
    }

    internal void Setup(Z80TestHarness testHarness)
    {
        testHarness.AF = AF;
        testHarness.BC = BC;
        testHarness.DE = DE;
        testHarness.HL = HL;
        testHarness.I = I;
        testHarness.R = R;
        testHarness.PC = PC;
        testHarness.SP = SP;
        testHarness.IX = IX;
        testHarness.IY = IY;
        testHarness.WZ = WZ;
        testHarness.ShadowAF = ShadowAF;
        testHarness.ShadowBC = ShadowBC;
        testHarness.ShadowDE = ShadowDE;
        testHarness.ShadowHL = ShadowHL;
        testHarness.IM = IM;
        testHarness.IFF1 = IFF1;
        testHarness.IFF2 = IFF2;
        testHarness.IsHalted = IsHalted;

        foreach (var memory in Memory)
        {
            var address = memory.Address;
            foreach (var data in memory.Data)
            {
                testHarness.SetMemory(address, data);
                address++;
            }
        }
    }
}