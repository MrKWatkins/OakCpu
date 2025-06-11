using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedDataMember : DataMember
{
    private UserDefinedDataMember(string name, DataType type, DataMemberVisibility visibility)
        : base(name, type, visibility)
    {
    }

    [Pure]
    public static IEnumerable<UserDefinedDataMember> Create([InstantHandle] IEnumerable<FieldYaml> yamls, DataMemberVisibility visibility) =>
        yamls.Select(y => new UserDefinedDataMember(y.Name, y.Type, visibility));
}