namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class Action(string name, int value)
{
    public static Action None { get; } = new("none", 0);

    public string Name { get; } = name;

    public int Value { get; } = value;

    public string EnumName => Name.ToUpperCamelCaseFromSnakeCase();

    public override string ToString() => Name;
}