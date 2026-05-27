using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorSerializationGenerator : SerializationGenerator
{
    public static readonly EmulatorSerializationGenerator Instance = new();

    private EmulatorSerializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.Emulator(context)}.serialization";

    [Pure]
    protected override string GetSerializedTypeName(GeneratorContext context) => Class.Name.Emulator(context);

    [Pure]
    protected override bool SerializesOverlapPipeline => true;
}