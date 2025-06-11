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
    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration)
    {
        var structLayout = CreateStructLayoutAttribute(requiredUsings);

        var members = input.Configuration.Registers.Values.Select(r => CreateField(requiredUsings, r)).ToList<MemberDeclarationSyntax>();

        members.Add(CreateConstructor(input));

        var fieldOffset = GetObjectPropertiesFieldOffset(input);
        members.Add(CreateGetOnlyProperty(requiredUsings, GetRegistersClassName(input), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(requiredUsings, GetFlagsClassName(input), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(requiredUsings, GetInterruptsClassName(input), InterruptsPropertyName, fieldOffset));
        fieldOffset += 8;

        // Order by size descending so each field ends up aligned to its own width.
        foreach (var dataMember in input.Configuration.AllDataMembers.Values.OrderByDescending(m => m.Size))
        {
            members.Add(CreateDataMember(requiredUsings, dataMember, fieldOffset));
            fieldOffset += dataMember.Size;
        }

        return classDeclaration
            .AddAttributeLists(AttributeList(SingletonSeparatedList(structLayout)))
            .AddMembers(members.ToArray<MemberDeclarationSyntax>());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input)
    {
        var statements = new List<StatementSyntax>
        {
            // opcodeStepTable = OpcodeStepTableNoPrefix;
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(PreDefinedDataMember.OpcodeStepTable.MemberName),
                    IdentifierName(input.Configuration.OpcodeStepTables.NoPrefix.FieldName))),

            // Registers = new Z80Registers(this);
            CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetRegistersClassName(input), ThisExpression()),

            // Flags = new Z80Flags(this);
            CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetFlagsClassName(input), ThisExpression()),

            // Interrupts = new Z80Interrupts(this);
            CreateNewObjectAndAssignToProperty(InterruptsPropertyName, GetInterruptsClassName(input), ThisExpression())
        };

        return ConstructorDeclaration(GetEmulatorClassName(input))
            .WithModifiers(TokenList(Public))
            .WithBody(Block(statements));
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateGetOnlyProperty(HashSet<string> requiredUsings, string typeName, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(requiredUsings, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Field));

        return CreateGetOnlyProperty(typeName, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static MemberDeclarationSyntax CreateDataMember(HashSet<string> requiredUsings, DataMember member, int fieldOffset) =>
        member.Visibility == DataMemberVisibility.Public
            ? CreateGetSetProperty(requiredUsings, member, fieldOffset)
            : CreateField(requiredUsings, member, fieldOffset, member.Visibility == DataMemberVisibility.Internal ? Internal : Private);

    [Pure]
    private static PropertyDeclarationSyntax CreateGetSetProperty(HashSet<string> requiredUsings, DataMember member, int fieldOffset) =>
        CreateGetSetProperty(requiredUsings, member.TypeSyntax, member.MemberName, fieldOffset);

    [Pure]
    private static PropertyDeclarationSyntax CreateGetSetProperty(HashSet<string> requiredUsings, TypeSyntax type, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(requiredUsings, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Field));

        return CreateGetSetProperty(type, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static int GetObjectPropertiesFieldOffset(GeneratorInput input)
    {
        var lastRegister = input.Configuration.Registers.Values.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();

        // Round up to the next multiple of 64 bits, i.e. 8 bytes.
        return (nextFieldOffset + 7) & ~7;
    }

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, Register register) =>
        CreateField(requiredUsings, register.Type.TypeSyntax(), register.FieldName, register.FieldOffset, Internal);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, DataMember member, int fieldOffset, SyntaxToken visibility, bool readOnly = false, ExpressionSyntax? initializer = null) =>
        CreateField(requiredUsings, member.TypeSyntax, member.MemberName, fieldOffset, visibility, readOnly, initializer);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, TypeSyntax type, string name, int fieldOffset, SyntaxToken visibility, bool readOnly = false, ExpressionSyntax? initializer = null)
    {
        var attribute = CreateFieldOffsetAttribute(requiredUsings, fieldOffset);

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
    private static AttributeSyntax CreateStructLayoutAttribute(HashSet<string> requiredUsings)
    {
        requiredUsings.Add("System.Runtime.InteropServices");

        return Attribute(
            IdentifierName("StructLayout"),
            AttributeArgumentList(
                SingletonSeparatedList(
                    AttributeArgument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("LayoutKind"),
                            IdentifierName("Explicit"))))));
    }

    [MustUseReturnValue]
    private static AttributeSyntax CreateFieldOffsetAttribute(HashSet<string> requiredUsings, int fieldOffset)
    {
        requiredUsings.Add("System.Runtime.InteropServices");

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