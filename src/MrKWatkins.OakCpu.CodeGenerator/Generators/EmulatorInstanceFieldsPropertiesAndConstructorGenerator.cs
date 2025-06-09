using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorInstanceFieldsPropertiesAndConstructorGenerator : EmulatorClassGenerator
{
    private const string RegistersPropertyName = "Registers";
    private const string FlagsPropertyName = "Flags";
    public static readonly EmulatorInstanceFieldsPropertiesAndConstructorGenerator Instance = new();

    private EmulatorInstanceFieldsPropertiesAndConstructorGenerator()
    {
    }

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration)
    {
        var structLayout = CreateStructLayoutAttribute(requiredUsings);

        var members = input.Registers.Values.Select(r => CreateField(requiredUsings, r)).ToList<MemberDeclarationSyntax>();

        members.Add(CreateConstructor(input));

        var fieldOffset = GetObjectPropertiesFieldOffset(input);
        members.Add(CreateGetOnlyProperty(requiredUsings, GetRegistersClassName(input), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(requiredUsings, GetFlagsClassName(input), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateField(requiredUsings, DataMember.OpcodeStepTable, fieldOffset, Private));
        fieldOffset += 8;

        members.Add(CreateGetSetProperty(requiredUsings, DataMember.Address, fieldOffset));
        fieldOffset += DataMember.Address.MemberSize;

        // TODO: Make private, think of a nice and quick way to indicate the end (or start!) of an instruction.
        members.Add(CreateField(requiredUsings, DataMember.Step, fieldOffset, Internal));
        fieldOffset += DataMember.Step.MemberSize;

        members.Add(CreateGetSetProperty(requiredUsings, DataMember.Data, fieldOffset));
        fieldOffset += DataMember.Data.MemberSize;

        members.Add(CreateGetSetProperty(requiredUsings, DataMember.Latch, fieldOffset));

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
                    IdentifierName(DataMember.OpcodeStepTable.Name),
                    IdentifierName(input.OpcodeStepTables.NoPrefix.FieldName))),

            // Registers = new Z80Registers(this);
            CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetRegistersClassName(input), ThisExpression()),

            // Flags = new Z80Flags(this);
            CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetFlagsClassName(input), ThisExpression())
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
    private static PropertyDeclarationSyntax CreateGetSetProperty(HashSet<string> requiredUsings, DataMember member, int fieldOffset) =>
        CreateGetSetProperty(requiredUsings, member.MemberTypeSyntax, member.Name, fieldOffset);

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
        var lastRegister = input.Registers.Values.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();

        // Round up to the next multiple of 64 bits, i.e. 8 bytes.
        return (nextFieldOffset + 7) & ~7;
    }

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, Register register) =>
        CreateField(requiredUsings, register.Type.TypeSyntax(), register.FieldName, register.FieldOffset, Internal);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, DataMember member, int fieldOffset, SyntaxToken visibility, bool readOnly = false, ExpressionSyntax? initializer = null) =>
        CreateField(requiredUsings, member.MemberTypeSyntax, member.Name, fieldOffset, visibility, readOnly, initializer);

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