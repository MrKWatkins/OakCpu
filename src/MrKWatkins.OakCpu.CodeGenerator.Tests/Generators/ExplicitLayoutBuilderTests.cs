using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class ExplicitLayoutBuilderTests : TestFixture
{
    [Test]
    public void CreateStructLayoutAttribute()
    {
        var context = CreateZ80FileGeneratorContext();

        var result = ExplicitLayoutBuilder.CreateStructLayoutAttribute(context);

        context.RequiredUsings.Contains("System.Runtime.InteropServices").Should().BeTrue();
        result.ToNormalizedString().Should().Equal("StructLayout(LayoutKind.Explicit)");
    }

    [Test]
    public void CreateFieldOffsetAttribute()
    {
        var context = CreateZ80FileGeneratorContext();

        var result = ExplicitLayoutBuilder.CreateFieldOffsetAttribute(context, 42);

        context.RequiredUsings.Contains("System.Runtime.InteropServices").Should().BeTrue();
        result.ToNormalizedString().Should().Equal("FieldOffset(42)");
    }

    [Test]
    public void CreateGetOnlyPropertyWithFieldOffset()
    {
        var context = CreateZ80FileGeneratorContext();

        var result = ExplicitLayoutBuilder.CreateGetOnlyPropertyWithFieldOffset(context, "Z80Registers", Identifiers.Property.Name.Registers, 24);

        context.RequiredUsings.Contains("System.Runtime.InteropServices").Should().BeTrue();
        context.RequiredUsings.Contains("System.Runtime.CompilerServices").Should().BeTrue();
        result.ToNormalizedString().Should().Contain("[field: FieldOffset(24)]");
        result.ToNormalizedString().Should().Contain("public Z80Registers Registers");
        result.ToNormalizedString().Should().Contain("get;");
    }

    [Test]
    public void CreateDataMemberProperty()
    {
        var context = CreateZ80FileGeneratorContext();

        var result = ExplicitLayoutBuilder.CreateDataMemberProperty(context, PreDefinedDataMember.Data);

        context.RequiredUsings.Contains("System.Runtime.CompilerServices").Should().BeTrue();
        result.ToNormalizedString().Should().Contain("public byte Data");
        result.ToNormalizedString().Should().Contain("get => data;");
        result.ToNormalizedString().Should().Contain("set => data = value;");
    }

    [Test]
    public void GetObjectPropertySummary() => ExplicitLayoutBuilder.GetObjectPropertySummary(Z80GeneratorContext, Identifiers.Property.Name.Interrupts).Should().Equal("Gets the Z80 interrupt state.");
}