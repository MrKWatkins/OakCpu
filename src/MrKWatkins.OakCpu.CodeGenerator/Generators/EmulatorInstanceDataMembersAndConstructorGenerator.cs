using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

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

        var fieldOffset = ExplicitLayoutBuilder.GetRegistersEndOffset(context);

        // Keep the serializable primitive state contiguous after the register block.
        foreach (var dataMember in generatorContext.Configuration.AllDataMembers.Values.Where(member => member != PreDefinedDataMember.OpcodeStepTable).OrderByDescending(member => member.Size))
        {
            members.AddRange(CreateDataMember(context, dataMember, fieldOffset));
            fieldOffset += dataMember.Size;
        }

        // The pins state is deliberately placed after the serialized members; it describes the most recent step only, so it is
        // not part of the serialized CPU state.
        if (generatorContext.Configuration.HasPins)
        {
            members.Add(ExplicitLayoutBuilder.CreateOffsetField(context, ByteType, Field.Name.Pins, fieldOffset, Private));
            members.Add(CreatePinsProperty(context));
            fieldOffset += 1;
        }

        fieldOffset = ExplicitLayoutBuilder.AlignReferenceFieldOffset(fieldOffset);
        members.Add(CreateObjectProperty(context, Class.Name.StepRegisters(context), Property.Name.Registers, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.StepFlags(context), Property.Name.Flags, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.StepInterrupts(context), Property.Name.Interrupts, fieldOffset));
        fieldOffset += 8;

        members.AddRange(CreateDataMember(context, PreDefinedDataMember.OpcodeStepTable, fieldOffset));
        fieldOffset += PreDefinedDataMember.OpcodeStepTable.Size;

        members.Add(CreateOverlapPipelineField(context, fieldOffset));

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
    private static PropertyDeclarationSyntax CreatePinsProperty(FileGeneratorContext context)
    {
        var pinsType = Class.Name.Pins(context.GeneratorContext);
        var getter = CreateGetAccessor(
            context.RequiredUsings,
            CastExpression(IdentifierName(pinsType), IdentifierName(Field.Name.Pins)));

        return WithXmlDocumentation(
            PropertyDeclaration(IdentifierName(pinsType), Identifier(Property.Name.Pins))
                .WithModifiers(TokenList(Public))
                .WithAccessorList(AccessorList([getter])),
            $"Gets the pins asserted during the T-state completed by the most recent call to Step, sampled at the rising edge of the T-state's clock. None after a reset, and undefined after a restore, until the next call to Step.");
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