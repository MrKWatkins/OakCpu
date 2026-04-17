using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

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

    protected override string GetBaseFileName(GeneratorContext context) => GetEmulatorClassName(context);

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
                    IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                    IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName))),

            // Registers = new Z80StepRegisters(this);
            CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetStepRegistersClassName(context), ThisExpression()),

            // Flags = new Z80StepFlags(this);
            CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetStepFlagsClassName(context), ThisExpression()),

            // Interrupts = new Z80StepInterrupts(this);
            CreateNewObjectAndAssignToProperty(InterruptsPropertyName, GetStepInterruptsClassName(context), ThisExpression())
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
                .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)])])
                .WithSemicolonToken(Semicolon)
        };

        if (member.SetterVisibility != null)
        {
            var setExpression = AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                fieldAccessExpression,
                IdentifierName("value"));

            var setter =
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(setExpression))
                    .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context.RequiredUsings, MethodImplOptions.AggressiveInlining)])])
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
    private static FieldDeclarationSyntax CreateOverlapPipelineField(GeneratorContext context, int fieldOffset) =>
        CreateField(context, CreateOverlapHandlerType(context), PreDefinedDataMember.OverlapPipeline.FieldName, fieldOffset, PreDefinedDataMember.OverlapPipeline.FieldVisibility.ToSyntax());

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