namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class DocumentationGeneratorTests : TestFixture
{
    [Test]
    public void Generate()
    {
        var documents = DocumentationGenerator.Generate(Z80Model);

        documents.Count.Should().Equal(Z80Documentation.Count);
        documents.Select(d => d.Path).Should().SequenceEqual(Z80Documentation.Select(d => d.Path));
        documents.Should().OnlyContain(d => d.Markdown.StartsWith("---\n", StringComparison.Ordinal));
    }
}