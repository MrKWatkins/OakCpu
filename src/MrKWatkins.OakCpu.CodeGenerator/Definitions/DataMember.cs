using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class DataMember
{
    private protected DataMember(string name, DataType type, DataMemberVisibility visibility = DataMemberVisibility.Private, bool isArray = false)
    {
        Name = name;
        Type = type;
        IsArray = isArray;
        Visibility = visibility;
        MemberName = Name.ToUpperCamelCaseFromSnakeCase();
        if (visibility != DataMemberVisibility.Public)
        {
            MemberName = new string(MemberName.TakeWhile(char.IsUpper).Select(char.ToLowerInvariant).Concat(MemberName.SkipWhile(char.IsUpper)).ToArray());
        }
    }

    public string Name { get; }

    public string MemberName { get; }

    public DataType Type { get; }

    public bool IsArray { get; }

    public DataMemberVisibility Visibility { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax(IsArray);

    public int Size => Type.Size(IsArray);

    public override string ToString() => Name;
}