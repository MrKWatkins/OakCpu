namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class DocumentationGeneratorTests : TestFixture
{
    [Test]
    public void Generate()
    {
        var documents = DocumentationGenerator.Generate(Z80Model);

        documents.Count.Should().Equal(Z80Documentation.Count + 3);
        documents[0].Path.Should().Equal(DocumentationGenerator.YamlFileName);
        documents[0].Content.Should().StartWith("cpu: Z80\n");
        documents[1].Path.Should().Equal(DocumentationGenerator.IndexFileName);
        documents[2].Path.Should().Equal(DocumentationGenerator.OpcodesFileName);
        documents.Skip(3).Select(d => d.Path).Should().SequenceEqual(Z80Documentation.Select(d => d.Page));
        documents.Skip(1).Should().OnlyContain(d => d.Content.StartsWith("# ", StringComparison.Ordinal));
    }
}