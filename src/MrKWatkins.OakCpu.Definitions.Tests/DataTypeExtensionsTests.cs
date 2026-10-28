namespace MrKWatkins.OakCpu.Definitions.Tests;

public sealed class DataTypeExtensionsTests
{
    [TestCase(DataType.Void, false, 0)]
    [TestCase(DataType.U8, false, 1)]
    [TestCase(DataType.I8, false, 1)]
    [TestCase(DataType.U16, false, 2)]
    [TestCase(DataType.I32, false, 4)]
    [TestCase(DataType.I32Bool, false, 4)]
    [TestCase(DataType.Bool, false, 1)]
    public void Size(DataType type, bool isArray, int expected) => type.Size(isArray).Should().Equal(expected);

    [TestCase(DataType.Void, true, 8)]
    [TestCase(DataType.U8, true, 8)]
    [TestCase(DataType.I8, true, 8)]
    [TestCase(DataType.U16, true, 8)]
    [TestCase(DataType.I32, true, 8)]
    [TestCase(DataType.I32Bool, true, 8)]
    [TestCase(DataType.Bool, true, 8)]
    public void Size_WithArray(DataType type, bool isArray, int expected) => type.Size(isArray).Should().Equal(expected);

    [Test]
    public void Size_ThrowsForUnsupportedType()
    {
        var invalidType = (DataType)99;
        invalidType.Invoking(t => t.Size()).Should().Throw<NotSupportedException>()
            .That.Should().HaveMessage("The DataType 99 is not supported.");
    }

    [TestCase(DataType.U8, false)]
    [TestCase(DataType.I8, true)]
    [TestCase(DataType.U16, false)]
    [TestCase(DataType.I32, true)]
    [TestCase(DataType.I32Bool, true)]
    public void IsSigned(DataType type, bool expected) => type.IsSigned.Should().Equal(expected);

    [Test]
    public void IsSigned_ThrowsForNonNumericType()
    {
        DataType.Bool.Invoking(t => _ = t.IsSigned).Should().Throw<NotSupportedException>()
            .That.Should().HaveMessage("The DataType Bool is not numeric.");
    }
}