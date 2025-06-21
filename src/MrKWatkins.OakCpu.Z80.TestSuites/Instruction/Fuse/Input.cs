namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public sealed class Input : FuseZ80State
{
    private Input()
    {
    }

    [Pure]
    internal static Input Parse(StreamReader reader)
    {
        var input = new Input();
        input.Parse(reader.ReadLine()!, reader);
        return input;
    }
}