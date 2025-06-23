namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public sealed class FuseZ80InputState : Z80InputState
{
    internal FuseZ80InputState()
    {
    }

    public ulong MinimumTStatesToRun { get; internal set; }
}