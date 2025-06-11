namespace MrKWatkins.OakCpu.Z80.TestSuites.InstructionLevel.Fuse;

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
        testHarness.RegisterAF = RegisterAF;
        testHarness.RegisterBC = RegisterBC;
        testHarness.RegisterDE = RegisterDE;
        testHarness.RegisterHL = RegisterHL;
        testHarness.RegisterI = RegisterI;
        testHarness.RegisterR = RegisterR;
        testHarness.RegisterPC = RegisterPC;
        testHarness.RegisterSP = RegisterSP;
        testHarness.RegisterIX = RegisterIX;
        testHarness.RegisterIY = RegisterIY;
        testHarness.RegisterWZ = RegisterWZ;
        testHarness.ShadowRegisterAF = ShadowRegisterAF;
        testHarness.ShadowRegisterBC = ShadowRegisterBC;
        testHarness.ShadowRegisterDE = ShadowRegisterDE;
        testHarness.ShadowRegisterHL = ShadowRegisterHL;
        testHarness.IM = IM;
        testHarness.IFF1 = IFF1;
        testHarness.IFF2 = IFF2;
        testHarness.IsHalted = IsHalted;

        foreach (var memory in Memory)
        {
            var address = memory.Address;
            foreach (var data in memory.Data)
            {
                testHarness.WriteByteToMemory(address, data);
                address++;
            }
        }
    }
}