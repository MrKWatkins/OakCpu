using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

// TODO: Skip requiredUsings, just add them, we know what they are.
public sealed class EmulatorInstanceDataMembersAndConstructorGenerator : EmulatorClassGenerator
{
    private const string RegistersPropertyName = "Registers";
    private const string FlagsPropertyName = "Flags";
    private const string InterruptsPropertyName = "Interrupts";
    public static readonly EmulatorInstanceDataMembersAndConstructorGenerator Instance = new();

    private EmulatorInstanceDataMembersAndConstructorGenerator()
    {
    }

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var structLayout = CreateStructLayoutAttribute(context);

        var members = context.Configuration.Registers.Values.Select(r => CreateField(context, r)).ToList<MemberDeclarationSyntax>();

        members.Add(CreateConstructor(context));

        var fieldOffset = GetObjectPropertiesFieldOffset(context);
        members.Add(CreateGetOnlyProperty(context, GetRegistersClassName(context), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(context, GetFlagsClassName(context), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(context, GetInterruptsClassName(context), InterruptsPropertyName, fieldOffset));
        fieldOffset += 8;

        // Order by size descending so each field ends up aligned to its own width.
        foreach (var dataMember in context.Configuration.AllDataMembers.Values.OrderByDescending(m => m.Size))
        {
            members.Add(CreateDataMember(context, dataMember, fieldOffset));
            fieldOffset += dataMember.Size;
        }

        return classDeclaration
            .AddAttributeLists(AttributeList(SingletonSeparatedList(structLayout)))
            .AddMembers(members.ToArray<MemberDeclarationSyntax>());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            // opcodeStepTable = OpcodeStepTableNoPrefix;
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(PreDefinedDataMember.OpcodeStepTable.MemberName),
                    IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName))),

            // Registers = new Z80Registers(this);
            CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetRegistersClassName(context), ThisExpression()),

            // Flags = new Z80Flags(this);
            CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetFlagsClassName(context), ThisExpression()),

            // Interrupts = new Z80Interrupts(this);
            CreateNewObjectAndAssignToProperty(InterruptsPropertyName, GetInterruptsClassName(context), ThisExpression())
        };

        return ConstructorDeclaration(GetEmulatorClassName(context))
            .WithModifiers(TokenList(Public))
            .WithBody(Block(statements));
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateGetOnlyProperty(GeneratorContext context, string typeName, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(context, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Field));

        return CreateGetOnlyProperty(context, typeName, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateDataMember(GeneratorContext context, DataMember member, int fieldOffset) =>
        member.Visibility == DataMemberVisibility.Public
            ? CreateGetSetProperty(context, member, fieldOffset)
            : CreateField(context, member, fieldOffset, member.Visibility == DataMemberVisibility.Internal ? Internal : Private);

    [Pure]
    private static PropertyDeclarationSyntax CreateGetSetProperty(GeneratorContext context, DataMember member, int fieldOffset) =>
        CreateGetSetProperty(context, member.TypeSyntax, member.MemberName, fieldOffset);

    [Pure]
    private static PropertyDeclarationSyntax CreateGetSetProperty(GeneratorContext context, TypeSyntax type, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(context, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Field));

        return CreateGetSetProperty(context, type, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static int GetObjectPropertiesFieldOffset(GeneratorContext context)
    {
        var lastRegister = context.Configuration.Registers.Values.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();

        // Round up to the next multiple of 64 bits, i.e. 8 bytes.
        return (nextFieldOffset + 7) & ~7;
    }

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(GeneratorContext context, Register register) =>
        CreateField(context, register.Type.TypeSyntax(), register.FieldName, register.FieldOffset, Internal);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(GeneratorContext context, DataMember member, int fieldOffset, SyntaxToken visibility, bool readOnly = false, ExpressionSyntax? initializer = null) =>
        CreateField(context, member.TypeSyntax, member.MemberName, fieldOffset, visibility, readOnly, initializer);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(GeneratorContext context, TypeSyntax type, string name, int fieldOffset, SyntaxToken visibility, bool readOnly = false, ExpressionSyntax? initializer = null)
    {
        var attribute = CreateFieldOffsetAttribute(context, fieldOffset);

        var modifiers = new List<SyntaxToken> { visibility };
        if (readOnly)
        {
            modifiers.Add(ReadOnly);
        }

        var variableDeclarator = VariableDeclarator(Identifier(name));


        if (initializer != null)
        {
            variableDeclarator = variableDeclarator.WithInitializer(EqualsValueClause(initializer));
        }

        var variable = VariableDeclaration(type).WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable)
            .AddAttributeLists(AttributeList(SingletonSeparatedList(attribute)))
            .AddModifiers(modifiers.ToArray());
    }

    [MustUseReturnValue]
    private static AttributeSyntax CreateStructLayoutAttribute(GeneratorContext context)
    {
        context.RequiredUsings.Add("System.Runtime.InteropServices");

        return Attribute(
            IdentifierName("StructLayout"),
            AttributeArgumentList(
                SingletonSeparatedList(
                    AttributeArgument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(nameof(LayoutKind)),
                            IdentifierName(nameof(LayoutKind.Explicit)))))));
    }

    [MustUseReturnValue]
    private static AttributeSyntax CreateFieldOffsetAttribute(GeneratorContext context, int fieldOffset)
    {
        context.RequiredUsings.Add("System.Runtime.InteropServices");

        return Attribute(
            IdentifierName("FieldOffset"),
            AttributeArgumentList(
                SingletonSeparatedList(
                    AttributeArgument(
                        LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            Literal(fieldOffset))))));
    }
}