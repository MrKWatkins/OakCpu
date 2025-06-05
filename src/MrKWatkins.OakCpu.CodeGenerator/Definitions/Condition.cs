namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Condition
{
    internal Condition(string name, Flag flag, bool isNot)
    {
        Name = name;
        Flag = flag;
        IsNot = isNot;
    }

    public string Name { get; }

    public Flag Flag { get; }

    public bool IsNot { get; }

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyDictionary<string, Condition> Create([InstantHandle] IEnumerable<Flag> flags)
    {
        var conditions = new Dictionary<string, Condition>();
        foreach (var flag in flags)
        {
            if (flag.Condition != null)
            {
                conditions.Add(flag.Condition, new Condition(flag.Condition, flag, false));
            }
            if (flag.NotCondition != null)
            {
                conditions.Add(flag.NotCondition, new Condition(flag.NotCondition, flag, true));
            }
        }

        return conditions;
    }
}