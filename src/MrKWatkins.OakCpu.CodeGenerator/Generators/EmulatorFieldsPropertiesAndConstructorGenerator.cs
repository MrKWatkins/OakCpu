using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorFieldsPropertiesAndConstructorGenerator : EmulatorClassGenerator
{
    private const string RegistersPropertyName = "Registers";
    private const string FlagsPropertyName = "Flags";
    public static readonly EmulatorFieldsPropertiesAndConstructorGenerator Instance = new();

    private EmulatorFieldsPropertiesAndConstructorGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration)
    {
        var structLayout = CreateStructLayoutAttribute(requiredUsings);

        var members = new List<MemberDeclarationSyntax>();
        members.AddRange(input.Registers.Select(r => CreateField(requiredUsings, r)));

        members.Add(CreateConstructor(input));

        var fieldOffset = GetObjectPropertiesFieldOffset(input);
        members.Add(CreateGetOnlyProperty(requiredUsings, GetRegistersClassName(input), RegistersPropertyName, fieldOffset));

        fieldOffset += 8;
        members.Add(CreateGetOnlyProperty(requiredUsings, GetFlagsClassName(input), FlagsPropertyName, fieldOffset));

        fieldOffset += 8;
        members.Add(CreateGetSetProperty(requiredUsings, UShort, AddressPropertyName, fieldOffset));

        fieldOffset += 2;
        members.Add(CreateGetSetProperty(requiredUsings, Byte, DataPropertyName, fieldOffset));

        fieldOffset += 1;
        members.Add(CreateField(requiredUsings, Byte, LastOpcodeFieldName, fieldOffset, Private));

        fieldOffset += 1;
        members.Add(CreateField(requiredUsings, UShort, StepIndexFieldName, fieldOffset, Private));

        return classDeclaration
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(structLayout)))
            .AddMembers(members.ToArray<MemberDeclarationSyntax>());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input)
    {
        var statements = new List<StatementSyntax>
        {
            // Registers = new Z80Registers(this);
            CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetRegistersClassName(input), SyntaxFactory.ThisExpression()),

            // Flags = new Z80Flags(this);
            CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetFlagsClassName(input), SyntaxFactory.ThisExpression())
        };

        return SyntaxFactory
            .ConstructorDeclaration(GetEmulatorClassName(input))
            .WithModifiers(SyntaxFactory.TokenList(Public))
            .WithBody(SyntaxFactory.Block(statements));
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateGetOnlyProperty(HashSet<string> requiredUsings, string typeName, string propertyName, int fieldOffset)
    {
        var attributeList = SyntaxFactory
            .AttributeList(SyntaxFactory.SingletonSeparatedList(CreateFieldOffsetAttribute(requiredUsings, fieldOffset)))
            .WithTarget(SyntaxFactory.AttributeTargetSpecifier(Field));

        return CreateGetOnlyProperty(typeName, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateGetSetProperty(HashSet<string> requiredUsings, TypeSyntax type, string propertyName, int fieldOffset)
    {
        var attributeList = SyntaxFactory
            .AttributeList(SyntaxFactory.SingletonSeparatedList(CreateFieldOffsetAttribute(requiredUsings, fieldOffset)))
            .WithTarget(SyntaxFactory.AttributeTargetSpecifier(Field));

        return CreateGetSetProperty(type, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static int GetObjectPropertiesFieldOffset(GeneratorInput input)
    {
        var lastRegister = input.Registers.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();

        // Round up to the next multiple of 64 bits, i.e. 8 bytes.
        return (nextFieldOffset + 7) & ~7;
    }

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, Register register) =>
        CreateField(requiredUsings, register.Type.PredefinedType(), register.FieldName, register.FieldOffset, Internal);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, PredefinedTypeSyntax type, string name, int fieldOffset, SyntaxToken visibility)
    {
        var attribute = CreateFieldOffsetAttribute(requiredUsings, fieldOffset);

        return SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(type)
                .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(name)))))
            .WithModifiers(SyntaxFactory.TokenList(visibility))
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute)));
    }

    [MustUseReturnValue]
    private static AttributeSyntax CreateStructLayoutAttribute(HashSet<string> requiredUsings)
    {
        requiredUsings.Add("System.Runtime.InteropServices");

        return SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName("StructLayout"),
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.AttributeArgument(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("LayoutKind"),
                            SyntaxFactory.IdentifierName("Explicit"))))));
    }
    [MustUseReturnValue]
    private static AttributeSyntax CreateFieldOffsetAttribute(HashSet<string> requiredUsings, int fieldOffset)
    {
        requiredUsings.Add("System.Runtime.InteropServices");

        return SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName("FieldOffset"),
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.AttributeArgument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(fieldOffset))))));
    }
}