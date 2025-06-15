using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedDataMember : DataMember
{
    private UserDefinedDataMember(string name, DataType type, Visibility fieldVisibility = Visibility.Private)
        : base(name, type, fieldVisibility)
    {
    }

    [Pure]
    public static IEnumerable<UserDefinedDataMember> Create([InstantHandle] IEnumerable<FieldYaml> yamls, Visibility visibility) =>
        yamls.Select(y => new UserDefinedDataMember(y.Name, y.Type, visibility));
}