using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratedNames;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the explicitly laid out step-emulator state fields, facade properties, and constructor.
/// </summary>
public sealed class EmulatorInstanceDataMembersAndConstructorGenerator : EmulatorClassGenerator
{
    /// <summary>
    /// The singleton instance of the generator.
    /// </summary>
    public static readonly EmulatorInstanceDataMembersAndConstructorGenerator Instance = new();

    private EmulatorInstanceDataMembersAndConstructorGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => GetEmulatorClassName(context);

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var members = context.Configuration.Registers.Values.Select(r => ExplicitLayoutBuilder.CreateRegisterField(context, r)).ToList<MemberDeclarationSyntax>();

        members.Add(CreateConstructor(context));

        var fieldOffset = ExplicitLayoutBuilder.GetObjectPropertiesFieldOffset(context);
        members.Add(CreateObjectProperty(context, GetRegistersClassName(context), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, GetFlagsClassName(context), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, GetInterruptsClassName(context), InterruptsPropertyName, fieldOffset));
        fieldOffset += 8;

        // Order by size descending so each field ends up aligned to its own width.
        foreach (var dataMember in context.Configuration.AllDataMembers.Values.Concat<DataMember>([PreDefinedDataMember.OverlapPipeline]).OrderByDescending(m => m == PreDefinedDataMember.OverlapPipeline ? 8 : m.Size))
        {
            if (dataMember == PreDefinedDataMember.OverlapPipeline)
            {
                members.Add(CreateOverlapPipelineField(context, fieldOffset));
                fieldOffset += 8;
            }
            else
            {
                members.AddRange(CreateDataMember(context, dataMember, fieldOffset));
                fieldOffset += dataMember.Size;
            }
        }

        return classDeclaration
            .AddAttributeLists(AttributeList(SingletonSeparatedList(ExplicitLayoutBuilder.CreateStructLayoutAttribute(context))))
            .AddMembers(members.ToArray<MemberDeclarationSyntax>());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context)
    {
        var statements = ExplicitLayoutBuilder.CreateConstructorStatements(
            context,
            [],
            (RegistersPropertyName, GetStepRegistersClassName(context)),
            (FlagsPropertyName, GetStepFlagsClassName(context)),
            (InterruptsPropertyName, GetStepInterruptsClassName(context)));

        return WithXmlDocumentation(
            ConstructorDeclaration(GetEmulatorClassName(context))
                .WithModifiers(TokenList(Public))
                .WithBody(Block(statements)),
            $"Initializes a new {GetEmulatorClassName(context)} instance.");
    }

    [Pure]
    private static IEnumerable<MemberDeclarationSyntax> CreateDataMember(GeneratorContext context, DataMember member, int fieldOffset)
    {
        yield return ExplicitLayoutBuilder.CreateOffsetField(context, member.TypeSyntax, member.FieldName, fieldOffset, member.FieldVisibility.ToSyntax());

        if (member.GetterVisibility == null)
        {
            yield break;
        }

        yield return WithXmlDocumentation(
            ExplicitLayoutBuilder.CreateDataMemberProperty(context, member),
            member.Documentation);
    }

    [Pure]
    private static FieldDeclarationSyntax CreateOverlapPipelineField(GeneratorContext context, int fieldOffset) =>
        ExplicitLayoutBuilder.CreateOffsetField(context, CreateOverlapHandlerType(context), PreDefinedDataMember.OverlapPipeline.FieldName, fieldOffset, PreDefinedDataMember.OverlapPipeline.FieldVisibility.ToSyntax());

    [Pure]
    private static PropertyDeclarationSyntax CreateObjectProperty(GeneratorContext context, string typeName, string propertyName, int fieldOffset) =>
        WithXmlDocumentation(
            ExplicitLayoutBuilder.CreateGetOnlyPropertyWithFieldOffset(context, typeName, propertyName, fieldOffset),
            ExplicitLayoutBuilder.GetObjectPropertySummary(context, propertyName));
}