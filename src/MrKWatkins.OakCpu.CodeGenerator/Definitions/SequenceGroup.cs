namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class SequenceGroup
{
    private SequenceGroup(string name, IReadOnlyDictionary<byte, StepSequence> members)
    {
        Name = name;
        Members = members;
    }

    public string Name { get; }

    public IReadOnlyDictionary<byte, StepSequence> Members { get; }

    public int MaximumNumber => Members.Keys.Max();

    [Pure]
    internal static IReadOnlyDictionary<string, SequenceGroup> Create(IEnumerable<(string GroupName, byte Number, StepSequence Sequence)> members)
    {
        var sequenceGroups = new Dictionary<string, SequenceGroup>(StringComparer.Ordinal);

        foreach (var group in members.GroupBy(member => member.GroupName, StringComparer.Ordinal))
        {
            sequenceGroups.Add(group.Key, new SequenceGroup(group.Key, group.ToDictionary(member => member.Number, member => member.Sequence)));
        }

        return sequenceGroups;
    }
}