using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorFieldsPropertiesAndConstructorGenerator : EmulatorClassGenerator
{
    private const string RegistersPropertyName = "Registers";
    private const string FlagsPropertyName = "Flags";
    public static readonly EmulatorFieldsPropertiesAndConstructorGenerator Instance = new();

    private EmulatorFieldsPropertiesAndConstructorGenerator()
    {
    }

    // TODO: An automatic layout algorithm taking into account padding would be nice.
    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration)
    {
        var structLayout = CreateStructLayoutAttribute(requiredUsings);

        var members = new List<MemberDeclarationSyntax>
        {
            CreateOpcodeStepTableField(input),
            CreateStaticConstructor(requiredUsings, input)
        };

        members.AddRange(input.Registers.Values.Select(r => CreateField(requiredUsings, r)));

        members.Add(CreateConstructor(input));

        var fieldOffset = GetObjectPropertiesFieldOffset(input);
        members.Add(CreateGetOnlyProperty(requiredUsings, GetRegistersClassName(input), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetOnlyProperty(requiredUsings, GetFlagsClassName(input), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateGetSetProperty(requiredUsings, DataMember.Address, fieldOffset));
        fieldOffset += DataMember.Address.MemberSize;

        members.Add(CreateGetSetProperty(requiredUsings, DataMember.Data, fieldOffset));
        fieldOffset += DataMember.Data.MemberSize;

        members.Add(CreateField(requiredUsings, DataMember.Opcode, fieldOffset, Private));
        fieldOffset += DataMember.Opcode.MemberSize;

        // TODO: Make private, think of a nice and quick way to indicate the end (or start!) of an instruction.
        members.Add(CreateField(requiredUsings, DataMember.Step, fieldOffset, Internal));

        return classDeclaration
            .AddAttributeLists(AttributeList(SingletonSeparatedList(structLayout)))
            .AddMembers(members.ToArray<MemberDeclarationSyntax>());
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input)
    {
        var statements = new List<StatementSyntax>
        {
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

    [Pure]
    private static FieldDeclarationSyntax CreateOpcodeStepTableField(GeneratorInput input)
    {
        var variableDeclarator = VariableDeclarator(Identifier(DataMember.OpcodeStepTable.Name))
            .WithInitializer(EqualsValueClause(CreateOpcodeStepTableInitializer(input)));

        var variable = VariableDeclaration(DataMember.OpcodeStepTable.MemberTypeSyntax)
            .WithVariables(SingletonSeparatedList(variableDeclarator));

        return FieldDeclaration(variable)
            .AddModifiers(Private, Static, ReadOnly);
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

    [Pure]
    private static ExpressionSyntax CreateOpcodeStepTableInitializer(GeneratorInput input)
    {
        var stepIndices = Enumerable.Repeat(65535, 256).ToArray();
        foreach (var instruction in input.Instructions)
        {
            stepIndices[instruction.Opcode] = instruction.Steps.First().Index;
        }

        var literals = stepIndices.Select(index => ExpressionElement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))));

        return CollectionExpression(SeparatedList<CollectionElementSyntax>(literals));
    }

    [MustUseReturnValue]
    private static ConstructorDeclarationSyntax CreateStaticConstructor(HashSet<string> requiredUsings, GeneratorInput input)
    {
        requiredUsings.Add(typeof(BitConverter).Namespace);
        requiredUsings.Add(typeof(NotSupportedException).Namespace);

        // throw new NotSupportedException("Only little endian systems are supported.");
        var throwStatement = ThrowStatement(
                ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("Only little endian systems are supported.")))))));

        // #pragma warning disable CA1065
        var pragmaDisable = PragmaWarningDirectiveTrivia(
                Token(SyntaxKind.DisableKeyword),
                SeparatedList<ExpressionSyntax>()
                    .Add(IdentifierName("CA1065")),
                true);

        // #pragma warning enable CA1065
        var pragmaRestore = PragmaWarningDirectiveTrivia(
                Token(SyntaxKind.RestoreKeyword),
                SeparatedList<ExpressionSyntax>()
                    .Add(IdentifierName("CA1065")),
                true);

        // !BitConverter.IsLittleEndian
        var condition = PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(BitConverter)),
                    IdentifierName(nameof(BitConverter.IsLittleEndian))));

        // if (!BitConverter.IsLittleEndian)
        // {
        // #pragma warning disable CA1065
        //  throw new NotSupportedException("Only little endian systems are supported.");
        // #pragma warning restore CA1065
        // }
        var ifStatement = IfStatement(
                condition,
                Block(
                    List<StatementSyntax>()
                        .Add(throwStatement
                            .WithLeadingTrivia(Trivia(pragmaDisable))
                            .WithTrailingTrivia(Trivia(pragmaRestore)))));

        // Static constructor
        return ConstructorDeclaration(GetEmulatorClassName(input))
            .WithModifiers(TokenList(Static))
            .WithBody(Block(SingletonList(ifStatement)));
    }
}