using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public abstract class DataMember
{
    private protected DataMember(string name, DataType type, bool isArray = false, bool isPublic = false)
    {
        Name = name;
        Type = type;
        IsArray = isArray;
        IsPublic = isPublic;
    }

    public string Name { get; }

    public DataType Type { get; }

    public bool IsArray { get; }

    public bool IsPublic { get; }

    public TypeSyntax TypeSyntax => Type.TypeSyntax(IsArray);

    public int Size => Type.Size(IsArray);
}