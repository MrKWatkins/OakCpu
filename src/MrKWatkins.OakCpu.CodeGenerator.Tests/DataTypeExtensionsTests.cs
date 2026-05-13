namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

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

    [TestCase(DataType.Void, false, "void")]
    [TestCase(DataType.U8, false, "byte")]
    [TestCase(DataType.I8, false, "sbyte")]
    [TestCase(DataType.U16, false, "ushort")]
    [TestCase(DataType.I32, false, "int")]
    [TestCase(DataType.I32Bool, false, "int")]
    [TestCase(DataType.Bool, false, "bool")]
    public void TypeSyntax(DataType type, bool isArray, string expected) => type.TypeSyntax(isArray).ToNormalizedString().Should().Equal(expected);

    [TestCase(DataType.Void, true, "void[]")]
    [TestCase(DataType.U8, true, "byte[]")]
    [TestCase(DataType.I8, true, "sbyte[]")]
    [TestCase(DataType.U16, true, "ushort[]")]
    [TestCase(DataType.I32, true, "int[]")]
    [TestCase(DataType.I32Bool, true, "int[]")]
    [TestCase(DataType.Bool, true, "bool[]")]
    public void TypeSyntax_WithArray(DataType type, bool isArray, string expected) => type.TypeSyntax(isArray).ToNormalizedString().Should().Equal(expected);

    [Test]
    public void TypeSyntax_ThrowsForUnsupportedType()
    {
        var invalidType = (DataType)99;
        invalidType.Invoking(t => t.TypeSyntax()).Should().Throw<NotSupportedException>()
            .That.Should().HaveMessage("The DataType 99 is not supported.");
    }

    [TestCase(DataType.U8, "0")]
    [TestCase(DataType.I8, "0")]
    [TestCase(DataType.U16, "0")]
    [TestCase(DataType.I32, "0")]
    [TestCase(DataType.I32Bool, "0")]
    [TestCase(DataType.Bool, "false")]
    public void DefaultLiteral(DataType type, string expected) => type.DefaultLiteral().ToNormalizedString().Should().Equal(expected);

    [Test]
    public void DefaultLiteral_ThrowsForUnsupportedType()
    {
        var invalidType = DataType.Void;
        invalidType.Invoking(t => t.DefaultLiteral()).Should().Throw<NotSupportedException>()
            .That.Should().HaveMessage("The DataType Void is not supported.");
    }

    [TestCase(DataType.U8, "ReadByte")]
    [TestCase(DataType.I8, "ReadSByte")]
    [TestCase(DataType.U16, "ReadUInt16")]
    [TestCase(DataType.I32, "ReadInt32")]
    [TestCase(DataType.I32Bool, "ReadInt32")]
    [TestCase(DataType.Bool, "ReadBoolean")]
    public void BinaryReaderMethodName(DataType type, string expected) => type.BinaryReaderMethodName().Should().Equal(expected);

    [Test]
    public void BinaryReaderMethodName_ThrowsForUnsupportedType()
    {
        var invalidType = DataType.Void;
        invalidType.Invoking(t => t.BinaryReaderMethodName()).Should().Throw<NotSupportedException>()
            .That.Should().HaveMessage("The DataType Void is not supported.");
    }
}