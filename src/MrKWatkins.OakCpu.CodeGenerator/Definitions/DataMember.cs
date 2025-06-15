using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class DataMember
{
    private protected DataMember(string name, DataType type, Visibility fieldVisibility = Visibility.Private, Visibility? getterVisibility = null, Visibility? setterVisibility = null, bool isArray = false)
    {
        if (getterVisibility is null && setterVisibility is not null)
        {
            throw new ArgumentException("Setter visibility must be null if getter visibility is null.");
        }

        Name = name;
        Type = type;
        FieldVisibility = fieldVisibility;
        IsArray = isArray;
        FieldName = name.ToLowerCamelCaseFromSnakeCase();
        PropertyName = FieldName.ToUpperCamelCaseFromSnakeCase();
        GetterVisibility = getterVisibility;
        SetterVisibility = setterVisibility;
    }

    public string Name { get; }

    public string FieldName { get; }

    public string PropertyName { get; }

    public DataType Type { get; }

    public bool IsArray { get; }

    public Visibility FieldVisibility { get; }

    public Visibility? GetterVisibility { get; }

    public Visibility? SetterVisibility { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax(IsArray);

    public int Size => Type.Size(IsArray);

    public override string ToString() => FieldName;
}