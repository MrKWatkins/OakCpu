using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedDataMember : DataMember
{
    private UserDefinedDataMember(string name, DataType type)
        : base(name, type)
    {
    }

    public override string ToString() => Name;

    [Pure]
    public static IReadOnlyDictionary<string, UserDefinedDataMember> Create(IReadOnlyList<FieldYaml> yamls) => yamls.Select(y => new UserDefinedDataMember(y.Name, y.Type)).ToDictionary(u => u.Name);
}