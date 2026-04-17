using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorStateGenerator : TypeGenerator
{
    private const string RegistersPropertyName = "Registers";
    private const string FlagsPropertyName = "Flags";
    private const string InterruptsPropertyName = "Interrupts";
    private const string PendingInterruptStepFieldName = "pendingInterruptStep";
    public static readonly InstructionEmulatorStateGenerator Instance = new();

    private InstructionEmulatorStateGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => GetInstructionEmulatorClassName(context);

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = context.Configuration.Registers.Values.Select(r => CreateField(context, r)).ToList<MemberDeclarationSyntax>();
        members.Add(CreateConstructor(context));

        var fieldOffset = GetObjectPropertiesFieldOffset(context);
        members.Add(CreateGetOnlyProperty(context, GetRegistersClassName(context), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(context, GetFlagsClassName(context), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(context, GetInterruptsClassName(context), InterruptsPropertyName, fieldOffset));
        fieldOffset += 8;

        foreach (var dataMember in context.Configuration.AllDataMembers.Values.Where(m => m != PreDefinedDataMember.CurrentStep).OrderByDescending(m => m.Size))
        {
            members.AddRange(CreateDataMember(context, dataMember, fieldOffset));
            fieldOffset += dataMember.Size;
        }

        if (fieldOffset % 2 != 0)
        {
            fieldOffset += 1;
        }

        members.Add(CreatePendingInterruptStepField(context, fieldOffset));

        return ClassDeclaration(GetInstructionEmulatorClassName(context))
            .AddModifiers(Public, Sealed, Unsafe, Partial)
            .AddAttributeLists(AttributeList(SingletonSeparatedList(CreateStructLayoutAttribute(context))))
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context) =>
        ConstructorDeclaration(GetInstructionEmulatorClassName(context))
            .WithModifiers(TokenList(Public))
            .WithBody(
                Block(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                            IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName))),
                    CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetInstructionRegistersClassName(context), ThisExpression()),
                    CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetInstructionFlagsClassName(context), ThisExpression()),
                    CreateNewObjectAndAssignToProperty(InterruptsPropertyName, GetInstructionInterruptsClassName(context), ThisExpression())));

    [Pure]
    private static IEnumerable<MemberDeclarationSyntax> CreateDataMember(GeneratorContext context, DataMember member, int fieldOffset)
    {
        yield return CreateField(context, member.TypeSyntax, member.FieldName, fieldOffset, member.FieldVisibility.ToSyntax());

        if (member.GetterVisibility == null)
        {
            yield break;
        }

        var fieldAccessExpression = IdentifierName(member.FieldName);

        var accessors = new List<AccessorDeclarationSyntax>
        {
            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(fieldAccessExpression))
                .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)])])
                .WithSemicolonToken(Semicolon)
        };

        if (member.SetterVisibility != null)
        {
            var setter = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithExpressionBody(
                    ArrowExpressionClause(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            fieldAccessExpression,
                            IdentifierName("value"))))
                .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)])])
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            if (member.SetterVisibility != member.GetterVisibility)
            {
                setter = setter.AddModifiers(member.SetterVisibility.Value.ToSyntax());
            }

            accessors.Add(setter);
        }

        yield return PropertyDeclaration(member.Type.TypeSyntax(), Identifier(member.PropertyName))
            .WithModifiers(TokenList(Public))
            .WithAccessorList(AccessorList(List(accessors)));
    }

    [Pure]
    private static FieldDeclarationSyntax CreatePendingInterruptStepField(GeneratorContext context, int fieldOffset) =>
        CreateField(context, UShortType, PendingInterruptStepFieldName, fieldOffset, Private);

    [Pure]
    private static PropertyDeclarationSyntax CreateGetOnlyProperty(GeneratorContext context, string typeName, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(context, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Field));

        return TypeGenerator.CreateGetOnlyProperty(context, typeName, propertyName).AddAttributeLists(attributeList);
    }

    [Pure]
    private static int GetObjectPropertiesFieldOffset(GeneratorContext context)
    {
        var lastRegister = context.Configuration.Registers.Values.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();
        return (nextFieldOffset + 7) & ~7;
    }

    [Pure]
    private static FieldDeclarationSyntax CreateField(GeneratorContext context, Register register) =>
        CreateField(context, register.Type.TypeSyntax(), register.FieldName, register.FieldOffset, Internal);

    [Pure]
    private static FieldDeclarationSyntax CreateField(GeneratorContext context, TypeSyntax type, string name, int fieldOffset, SyntaxToken visibility, bool readOnly = false, ExpressionSyntax? initializer = null)
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

    [Pure]
    private static AttributeSyntax CreateStructLayoutAttribute(GeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(LayoutKind).Namespace!);

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

    [Pure]
    private static AttributeSyntax CreateFieldOffsetAttribute(GeneratorContext context, int fieldOffset)
    {
        context.RequiredUsings.Add(typeof(FieldOffsetAttribute).Namespace!);

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