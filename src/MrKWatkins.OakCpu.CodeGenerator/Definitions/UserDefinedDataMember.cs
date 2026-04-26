using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedDataMember : DataMember
{
    private UserDefinedDataMember(string name, DataType type, Visibility fieldVisibility, Visibility? getterVisibility, Visibility? setterVisibility, Documentation documentation)
        : base(name, type, fieldVisibility, getterVisibility, setterVisibility, documentation: documentation)
    {
    }

    [Pure]
    public static IEnumerable<UserDefinedDataMember> Create([InstantHandle] IEnumerable<FieldYaml> yamls, Visibility visibility) =>
        yamls.Select(y => new UserDefinedDataMember(
            y.Name,
            y.Type,
            visibility,
            y.Getter ? Visibility.Public : null,
            y.Setter ? Visibility.Public : null,
            y.Getter ? Documentation.Create(y.Documentation, $"field {y.Name}") : Documentation.CreateOptional(y.Documentation, $"field {y.Name}")));
}