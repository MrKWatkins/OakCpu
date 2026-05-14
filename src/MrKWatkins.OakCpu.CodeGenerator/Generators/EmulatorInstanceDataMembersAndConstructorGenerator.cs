using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

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

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.Emulator(context);

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(FileGeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var generatorContext = context.GeneratorContext;
        var members = generatorContext.Configuration.Registers.Values.Select(r => ExplicitLayoutBuilder.CreateRegisterField(context, r)).ToList<MemberDeclarationSyntax>();

        members.Add(CreateConstructor(context));

        var fieldOffset = ExplicitLayoutBuilder.GetObjectPropertiesFieldOffset(context);
        members.Add(CreateObjectProperty(context, Class.Name.Registers(context), Property.Name.Registers, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.Flags(context), Property.Name.Flags, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.Interrupts(context), Property.Name.Interrupts, fieldOffset));
        fieldOffset += 8;

        // Order by size descending so each field ends up aligned to its own width.
        foreach (var dataMember in generatorContext.Configuration.AllDataMembers.Values.Concat<DataMember>([PreDefinedDataMember.OverlapPipeline]).OrderByDescending(m => m == PreDefinedDataMember.OverlapPipeline ? 8 : m.Size))
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
            (Property.Name.Registers, Class.Name.StepRegisters(context)),
            (Property.Name.Flags, Class.Name.StepFlags(context)),
            (Property.Name.Interrupts, Class.Name.StepInterrupts(context)));

        return WithXmlDocumentation(
            ConstructorDeclaration(Class.Name.Emulator(context))
                .WithModifiers(TokenList(Public))
                .WithBody(Block(statements)),
            $"Initializes a new {Class.Name.Emulator(context)} instance.");
    }

    [MustUseReturnValue]
    private static IEnumerable<MemberDeclarationSyntax> CreateDataMember(FileGeneratorContext context, DataMember member, int fieldOffset)
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

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateOverlapPipelineField(FileGeneratorContext context, int fieldOffset) =>
        ExplicitLayoutBuilder.CreateOffsetField(context, CreateOverlapHandlerType(context), PreDefinedDataMember.OverlapPipeline.FieldName, fieldOffset, PreDefinedDataMember.OverlapPipeline.FieldVisibility.ToSyntax());

    [MustUseReturnValue]
    private static PropertyDeclarationSyntax CreateObjectProperty(FileGeneratorContext context, string typeName, string propertyName, int fieldOffset) =>
        WithXmlDocumentation(
            ExplicitLayoutBuilder.CreateGetOnlyPropertyWithFieldOffset(context, typeName, propertyName, fieldOffset),
            ExplicitLayoutBuilder.GetObjectPropertySummary(context, propertyName));
}