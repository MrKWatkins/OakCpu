using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Definitions;

public sealed class VisibilityExtensionsTests
{
    [TestCase(Visibility.Private, "private")]
    [TestCase(Visibility.Internal, "internal")]
    [TestCase(Visibility.Public, "public")]
    public void ToSyntax(Visibility visibility, string expected)
    {
        visibility.ToSyntax().Text.Should().Equal(expected);
    }

    [Test]
    public void ToSyntax_ThrowsForUnsupportedVisibility()
    {
        var exception = Assert.Throws<NotSupportedException>(() => _ = ((Visibility)999).ToSyntax());

        exception!.Message.Should().Contain("not supported");
    }
}