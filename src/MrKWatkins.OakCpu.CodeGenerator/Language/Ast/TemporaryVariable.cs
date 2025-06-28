using System.Collections.Frozen;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class TemporaryVariable
{
    private static readonly FrozenDictionary<(DataType X, DataType Y), DataType> TypeResolutions = new Dictionary<(DataType X, DataType Y), DataType>
    {
        [(DataType.U16, DataType.U8)] = DataType.U16,
        [(DataType.U8, DataType.U16)] = DataType.U16,
        [(DataType.I32, DataType.U8)] = DataType.I32,
        [(DataType.U8, DataType.I32)] = DataType.I32,
        [(DataType.I32, DataType.U16)] = DataType.I32,
        [(DataType.U16, DataType.I32)] = DataType.I32

    }.ToFrozenDictionary();

    private DataType? type;

    internal TemporaryVariable(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public DataType Type
    {
        get => type ?? throw new InvalidOperationException($"The type of temporary variable ${Name} has not been resolved.");
        set => type = type.HasValue ? ResolveType(type.Value, value) : value;
    }

    [Pure]
    private DataType ResolveType(DataType currentType, DataType newType)
    {
        if (currentType == newType)
        {
            return currentType;
        }

        if (TypeResolutions.TryGetValue((currentType, newType), out var resolvedType))
        {
            return resolvedType;
        }

        throw new InvalidOperationException($"The type of temporary variable ${Name} has already been resolved to {currentType}, and cannot be changed to {newType}.");
    }

    public override string ToString() => $"{(type.HasValue ? type.ToString() : "???")} ${Name}";
}