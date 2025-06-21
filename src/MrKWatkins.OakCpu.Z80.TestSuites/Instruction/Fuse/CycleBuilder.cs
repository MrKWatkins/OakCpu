namespace MrKWatkins.OakCpu.Z80.TestSuites.Instruction.Fuse;

public static class CycleBuilder
{
    [Pure]
    public static IEnumerable<(int TState, Cycle Cycle)> BuildCycles([InstantHandle] IEnumerable<FuseEvent> fuseEvents, MemoryCycleMethod memoryCycleMethod)
    {
        var tStates = 0;
        foreach (var @event in fuseEvents)
        {
            if (@event.Type is not (FuseEventType.MemoryContend or FuseEventType.PortContend))
            {
                var cycleType = GetCycleType(@event.Type);

                if (memoryCycleMethod == MemoryCycleMethod.Start || @event.Type == FuseEventType.PortRead || @event.Type == FuseEventType.PortWrite)
                {
                    yield return (tStates, new Cycle(cycleType, @event.Address, @event.Data));
                }
                else if (memoryCycleMethod == MemoryCycleMethod.End)
                {
                    yield return (tStates + 1, new Cycle(cycleType, @event.Address, @event.Data));
                }
                else if (memoryCycleMethod == MemoryCycleMethod.Accurate)
                {
                    if (@event.Type == FuseEventType.MemoryRead)
                    {
                        yield return (tStates, new Cycle(cycleType, @event.Address, @event.Data));
                    }
                    yield return (tStates + 1, new Cycle(cycleType, @event.Address, @event.Data));
                }
                else
                {
                    throw new NotSupportedException($"The {nameof(MemoryCycleMethod)} {memoryCycleMethod} is not supported.");
                }
            }

            tStates = @event.TStatesAfter;
        }
    }

    [Pure]
    private static CycleType GetCycleType(FuseEventType type) => type switch
    {
        FuseEventType.MemoryRead => CycleType.MemoryRead,
        FuseEventType.MemoryWrite => CycleType.MemoryWrite,
        FuseEventType.PortWrite => CycleType.IOWrite,
        FuseEventType.PortRead => CycleType.IORead,
        _ => throw new NotSupportedException($"The {nameof(FuseEventType)} {type} is not supported.")
    };
}