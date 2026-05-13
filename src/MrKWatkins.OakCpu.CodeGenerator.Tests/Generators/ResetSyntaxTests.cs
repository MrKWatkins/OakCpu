using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class ResetSyntaxTests
{
    [TestCase(DataType.U8, "value = 0;")]
    [TestCase(DataType.U16, "value = 0;")]
    [TestCase(DataType.Bool, "value = false;")]
    public void GenerateReset(DataType type, string expected) => ResetSyntax.GenerateReset("value", type).ToNormalizedString().Should().Equal(expected);
}