using Microsoft.CodeAnalysis;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class TypeGeneratorTests : TestFixture
{
    [Test]
    public void AllGeneratorsContainsExpectedGenerators()
    {
        var allGenerators = TypeGenerator.AllGenerators;

        allGenerators.Should().NotBeNull();
        (allGenerators.Count > 0).Should().BeTrue();
        allGenerators.Contains(ActionRequiredGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(EmulatorOverlapsGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(StepStructGenerator.Instance).Should().BeTrue();
        allGenerators.Contains(RegistersClassesGenerator.Instance).Should().BeTrue();
    }

    [Test]
    public void InstructionHandlerMethodParameter()
    {
        var result = InstructionHandlerSyntax.MethodParameter;

        result.ToNormalizedString().Should().Equal("ref THandler handler");
    }

    [Test]
    public void GetFileName()
    {
        new TestTypeGenerator().GetFileName(Z80GeneratorContext).Should().Equal("TestFile.generated.cs");
    }

    [Test]
    public void GenerateFiles_DefaultImplementation()
    {
        var file = new TestTypeGenerator().GenerateFiles(Z80GeneratorContext).Single();

        file.FileName.Should().Equal("TestFile.generated.cs");
        file.Source.Should().Contain("class GeneratedType");
    }

    [Test]
    public void GenerateFiles_ThrowsWhenNeitherCreateTypeNorCreateTypesIsOverridden()
    {
        var exception = Assert.Throws<NotImplementedException>(() => _ = new ThrowingTypeGenerator().GenerateCompilationUnit(Z80GeneratorContext));

        exception!.Message.Should().Contain("CreateType");
    }

    [Test]
    public void WithXmlDocumentation_AddsRemarksParametersAndReturns()
    {
        var method = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "Test").AddAttributeLists(AttributeList());

        var documented = TestTypeGenerator.AddDocumentation(
            method,
            "Summary",
            "Remarks",
            new Dictionary<string, string> { ["value"] = "Parameter." },
            "Nothing.");

        var text = documented.ToFullString();
        text.Should().Contain("/// <summary>");
        text.Should().Contain("/// <remarks>");
        text.Should().Contain("/// <param name=\"value\">");
        text.Should().Contain("/// <returns>");
    }

    [Test]
    public void WithXmlDocumentation_DocumentationEmpty_ReturnsOriginalNode()
    {
        var property = PropertyDeclaration(IdentifierName("int"), "Value");

        ReferenceEquals(TestTypeGenerator.AddDocumentation(property, Documentation.Empty), property).Should().BeTrue();
    }

    private sealed class TestTypeGenerator : TypeGenerator
    {
        protected override string GetBaseFileName(GeneratorContext context) => "TestFile";

        protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context) => ClassDeclaration("GeneratedType");

        [Pure]
        public static T AddDocumentation<T>(T node, Documentation documentation)
            where T : SyntaxNode =>
            WithXmlDocumentation(node, documentation);

        [Pure]
        public static T AddDocumentation<T>(T node, string summary, string? remarks, IReadOnlyDictionary<string, string>? parameters, string? returns)
            where T : SyntaxNode =>
            WithXmlDocumentation(node, summary, remarks, parameters, returns);
    }

    private sealed class ThrowingTypeGenerator : TypeGenerator
    {
        protected override string GetBaseFileName(GeneratorContext context) => "Throwing";
    }
}