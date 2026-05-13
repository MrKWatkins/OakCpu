using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Builds explicit-layout fields, properties, and constructor statements shared by the emulator state generators.
/// </summary>
internal static class ExplicitLayoutBuilder
{
    /// <summary>
    /// Creates the common constructor statements that initialize the opcode table and object-backed facade properties.
    /// </summary>
    [Pure]
    public static IEnumerable<StatementSyntax> CreateConstructorStatements(
        GeneratorContext context,
        IEnumerable<StatementSyntax> additionalStatements,
        params (string PropertyName, string TypeName)[] objectProperties)
    {
        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName)));

        foreach (var statement in additionalStatements)
        {
            yield return statement;
        }

        foreach (var (propertyName, typeName) in objectProperties)
        {
            yield return CreateNewObjectAndAssignToProperty(propertyName, typeName, ThisExpression());
        }
    }

    /// <summary>
    /// Creates a <c>StructLayout(LayoutKind.Explicit)</c> attribute and records the required using directive.
    /// </summary>
    [MustUseReturnValue]
    public static AttributeSyntax CreateStructLayoutAttribute(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(LayoutKind));

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

    /// <summary>
    /// Creates a <c>FieldOffset</c> attribute for an explicitly laid out field and records the required using directive.
    /// </summary>
    [MustUseReturnValue]
    public static AttributeSyntax CreateFieldOffsetAttribute(FileGeneratorContext context, int fieldOffset)
    {
        context.RequiredUsings.Add(typeof(FieldOffsetAttribute));

        return Attribute(
            IdentifierName("FieldOffset"),
            AttributeArgumentList(
                SingletonSeparatedList(
                    AttributeArgument(
                        LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            Literal(fieldOffset))))));
    }

    /// <summary>
    /// Creates an explicitly laid out field for a register.
    /// </summary>
    [MustUseReturnValue]
    public static FieldDeclarationSyntax CreateRegisterField(FileGeneratorContext context, Register register) =>
        CreateOffsetField(context, register.Type.TypeSyntax(), register.FieldName, register.FieldOffset, Internal);

    /// <summary>
    /// Creates an explicitly laid out field declaration.
    /// </summary>
    [MustUseReturnValue]
    public static FieldDeclarationSyntax CreateOffsetField(
        FileGeneratorContext context,
        TypeSyntax type,
        string name,
        int fieldOffset,
        SyntaxToken visibility,
        bool readOnly = false,
        ExpressionSyntax? initializer = null)
    {
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

        return FieldDeclaration(VariableDeclaration(type).WithVariables(SingletonSeparatedList(variableDeclarator)))
            .AddAttributeLists(AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(context, fieldOffset))))
            .AddModifiers(modifiers.ToArray());
    }

    /// <summary>
    /// Creates a get-only property backed by a field-targeted <c>FieldOffset</c> attribute.
    /// </summary>
    [MustUseReturnValue]
    public static PropertyDeclarationSyntax CreateGetOnlyPropertyWithFieldOffset(FileGeneratorContext context, string typeName, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(context, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.FieldKeyword)));

        return PropertyDeclaration(IdentifierName(typeName), Identifier(propertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(AccessorList([CreateGetAccessor(context.RequiredUsings)]))
            .AddAttributeLists(attributeList);
    }

    /// <summary>
    /// Creates a property that exposes a data member field through aggressive-inlined accessors.
    /// </summary>
    [MustUseReturnValue]
    public static PropertyDeclarationSyntax CreateDataMemberProperty(FileGeneratorContext context, DataMember member)
    {
        var fieldAccessExpression = IdentifierName(member.FieldName);

        var accessors = new List<AccessorDeclarationSyntax>
        {
            CreateGetAccessor(context.RequiredUsings, fieldAccessExpression)
        };

        if (member.SetterVisibility != null)
        {
            SyntaxTokenList? setterModifiers = member.SetterVisibility != member.GetterVisibility
                ? TokenList(member.SetterVisibility.Value.ToSyntax())
                : null;

            accessors.Add(
                CreateSetAccessor(
                    context.RequiredUsings,
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        fieldAccessExpression,
                        IdentifierName("value")),
                    setterModifiers));
        }

        return PropertyDeclaration(member.Type.TypeSyntax(), Identifier(member.PropertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(AccessorList(List(accessors)));
    }

    /// <summary>
    /// Calculates the first aligned offset after the register block where object-backed properties can be placed.
    /// </summary>
    [Pure]
    public static int GetObjectPropertiesFieldOffset(GeneratorContext context)
    {
        var lastRegister = context.Configuration.Registers.Values.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();

        return (nextFieldOffset + 7) & ~7;
    }

    /// <summary>
    /// Gets the XML documentation summary for one of the generated object-backed facade properties.
    /// </summary>
    [Pure]
    public static string GetObjectPropertySummary(GeneratorContext context, string propertyName) => propertyName switch
    {
        Property.Name.Registers => $"Gets the {context.Cpu.Name} registers.",
        Property.Name.Flags => $"Gets the {context.Cpu.Name} flags.",
        Property.Name.Interrupts => $"Gets the {context.Cpu.Name} interrupt state.",
        _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
    };
}