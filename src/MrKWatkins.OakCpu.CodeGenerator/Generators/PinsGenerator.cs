using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the flags enum describing the CPU bus control pins asserted during a T-state. Only generated for CPUs that
/// define pins.
/// </summary>
public sealed class PinsGenerator : TypeGenerator
{
    public static readonly PinsGenerator Instance = new();

    private PinsGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.Pins(context);

    public override IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) =>
        context.Configuration.HasPins ? base.GenerateFiles(context) : [];

    protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(FlagsAttribute));

        var none = WithXmlDocumentation(
            EnumMemberDeclaration([], Identifier("None"), EqualsValueClause(GenerateNumericLiteralExpression(0))),
            "No pins are asserted.");

        var members = context.GeneratorContext.Configuration.Pins.Values
            .OrderBy(pin => pin.Value)
            .Select(pin => WithXmlDocumentation(
                EnumMemberDeclaration([], Identifier(pin.Name), EqualsValueClause(GenerateNumericLiteralExpression(pin.Value))),
                pin.Documentation))
            .Prepend(none)
            .ToArray();

        return WithXmlDocumentation(
            EnumDeclaration(Class.Name.Pins(context.GeneratorContext))
                .AddModifiers(Public)
                .AddAttributeLists(AttributeList([Attribute(IdentifierName("Flags"))]))
                .WithBaseList(BaseList([SimpleBaseType(ByteType)]))
                .AddMembers(members),
            $"Describes the {context.GeneratorContext.Cpu.Name} bus control pins asserted during a T-state, sampled at the rising edge of the T-state's clock. Active-low pins are represented as active-high flags.");
    }
}