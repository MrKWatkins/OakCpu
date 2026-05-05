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
    public static readonly InstructionEmulatorStateGenerator Instance = new();

    private InstructionEmulatorStateGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => GetInstructionEmulatorClassName(context);

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = context.Configuration.Registers.Values.Select(r => CreateField(context, r)).ToList<MemberDeclarationSyntax>();
        members.Add(CreateNoNextSequenceStepField());
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

        members.Add(CreateNextSequenceStepField(context, fieldOffset));
        members.Add(CreateExecuteInstructionMethod());
        members.Add(CreateExecuteDecodedInstructionMethod(context));
        members.Add(CreateCompleteInstructionMethod(context));

        return WithXmlDocumentation(
            ClassDeclaration(GetInstructionEmulatorClassName(context))
                .AddModifiers(Public, Sealed, Unsafe, Partial)
                .AddAttributeLists(AttributeList(SingletonSeparatedList(CreateStructLayoutAttribute(context))))
                .AddMembers(members.ToArray()),
            $"Represents a {context.Cpu.Name} emulator that executes one complete instruction at a time.");
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context) =>
        WithXmlDocumentation(
            ConstructorDeclaration(GetInstructionEmulatorClassName(context))
                .WithModifiers(TokenList(Public))
                .WithBody(
                    Block(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName),
                                IdentifierName(context.Configuration.OpcodeStepTables.NoPrefix.FieldName))),
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(NextSequenceStepFieldName),
                                IdentifierName(NoNextSequenceStepFieldName))),
                        CreateNewObjectAndAssignToProperty(RegistersPropertyName, GetInstructionRegistersClassName(context), ThisExpression()),
                        CreateNewObjectAndAssignToProperty(FlagsPropertyName, GetInstructionFlagsClassName(context), ThisExpression()),
                        CreateNewObjectAndAssignToProperty(InterruptsPropertyName, GetInstructionInterruptsClassName(context), ThisExpression()))),
            $"Initializes a new {GetInstructionEmulatorClassName(context)} instance.");

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

        yield return WithXmlDocumentation(
            PropertyDeclaration(member.Type.TypeSyntax(), Identifier(member.PropertyName))
                .WithModifiers(TokenList(Public))
                .WithAccessorList(AccessorList(List(accessors))),
            member.Documentation);
    }

    [Pure]
    private static FieldDeclarationSyntax CreateNoNextSequenceStepField() =>
        FieldDeclaration(
                VariableDeclaration(UShortType)
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(NoNextSequenceStepFieldName)
                                .WithInitializer(
                                    EqualsValueClause(
                                        ParseExpression("ushort.MaxValue"))))))
            .WithModifiers(TokenList(Internal, Token(SyntaxKind.ConstKeyword)));

    [Pure]
    private static FieldDeclarationSyntax CreateNextSequenceStepField(GeneratorContext context, int fieldOffset) =>
        CreateField(context, UShortType, NextSequenceStepFieldName, fieldOffset, Internal);

    [Pure]
    private static MethodDeclarationSyntax CreateExecuteInstructionMethod() =>
        WithXmlDocumentation(
            MethodDeclaration(IntType, Identifier("ExecuteInstruction"))
                .WithModifiers(TokenList(Public))
                .WithParameterList(ParameterList([CreateInstructionActionCallbackParameter()]))
                .WithBody(
                    Block(
                        List<StatementSyntax>(
                        [
                            ExpressionStatement(
                                InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(nameof(ArgumentNullException)),
                                            IdentifierName(nameof(ArgumentNullException.ThrowIfNull))))
                                    .WithArgumentList(
                                        ArgumentList(
                                        [
                                            Argument(IdentifierName(InstructionActionCallbackParameterName))
                                        ]))),

                            InitializeVariableStatement("scheduledSequenceStep", IdentifierName(NextSequenceStepFieldName), UShortType),
                            IfStatement(
                                BinaryExpression(
                                    SyntaxKind.NotEqualsExpression,
                                    IdentifierName("scheduledSequenceStep"),
                                    IdentifierName(NoNextSequenceStepFieldName)),
                                Block(
                                    List<StatementSyntax>(
                                    [
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(NextSequenceStepFieldName),
                                                IdentifierName(NoNextSequenceStepFieldName))),
                                        ReturnStatement(
                                            InvocationExpression(IdentifierName(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                    [
                                                        Argument(IdentifierName("scheduledSequenceStep")),
                                                        Argument(IdentifierName(InstructionActionCallbackParameterName))
                                                    ])))
                                    ]))),

                            ReturnStatement(
                                InvocationExpression(IdentifierName(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                        [
                                            Argument(IdentifierName(InstructionEmulatorGenerator.OpcodeReadStep0FieldName)),
                                            Argument(IdentifierName(InstructionActionCallbackParameterName))
                                        ])))
                        ]))),
            "Executes one complete instruction or scheduled sequence.",
            parameters: new Dictionary<string, string>
            {
                ["onActionRequired"] = "Called whenever the emulator requires an external bus action."
            },
            returns: "The number of T-states executed.");

    [Pure]
    private static MemberDeclarationSyntax CreateExecuteDecodedInstructionMethod(GeneratorContext context) =>
        MethodDeclaration(IntType, Identifier(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
            .AddAttributeLists(AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]))
            .WithModifiers(TokenList(Private))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier("decodedStep")).WithType(UShortType),
                    CreateInstructionActionCallbackParameter()
                ]))
            .WithBody(
                Block(
                    List<StatementSyntax>(
                    [
                        InitializeVariableStatement(
                            "instruction",
                            ElementAccessExpression(IdentifierName(InstructionHandlersFieldName))
                                .WithArgumentList(
                                    BracketedArgumentList(
                                    [
                                        Argument(IdentifierName("decodedStep"))
                                    ]))),
                        ReturnStatement(
                            InvocationExpression(IdentifierName("instruction"))
                                .WithArgumentList(
                                    ArgumentList(
                                    [
                                        Argument(ThisExpression()),
                                        Argument(IdentifierName(InstructionActionCallbackParameterName))
                                    ])))
                    ])));

    [Pure]
    private static MemberDeclarationSyntax CreateCompleteInstructionMethod(GeneratorContext context)
    {
        var statements = new List<StatementSyntax>
        {
            InitializeVariableStatement(EmulatorParameterName, ThisExpression())
        };
        statements.AddRange(StatementGenerator.GenerateInstructionCompletionStatements(context, context.OnInstructionComplete, "instructionUpdatesFlags"));
        statements.Add(ReturnStatement(IdentifierName("tStates")));

        return MethodDeclaration(IntType, Identifier(CompleteInstructionMethodName))
            .AddAttributeLists(AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]))
            .WithModifiers(TokenList(Private))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier("instructionUpdatesFlags")).WithType(BoolType),
                    Parameter(Identifier("tStates")).WithType(IntType)
                ]))
            .WithBody(Block(List(statements)));
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateGetOnlyProperty(GeneratorContext context, string typeName, string propertyName, int fieldOffset)
    {
        var attributeList = AttributeList(SingletonSeparatedList(CreateFieldOffsetAttribute(context, fieldOffset)))
            .WithTarget(AttributeTargetSpecifier(Field));

        return WithXmlDocumentation(
            TypeGenerator.CreateGetOnlyProperty(context, typeName, propertyName).AddAttributeLists(attributeList),
            GetObjectPropertySummary(context, propertyName));
    }

    [Pure]
    private static int GetObjectPropertiesFieldOffset(GeneratorContext context)
    {
        var lastRegister = context.Configuration.Registers.Values.OrderByDescending(r => r.FieldOffset).First();
        var nextFieldOffset = lastRegister.FieldOffset + lastRegister.Type.Size();
        return (nextFieldOffset + 7) & ~7;
    }

    [Pure]
    private static string GetObjectPropertySummary(GeneratorContext context, string propertyName) => propertyName switch
    {
        RegistersPropertyName => $"Gets the {context.Cpu.Name} registers.",
        FlagsPropertyName => $"Gets the {context.Cpu.Name} flags.",
        InterruptsPropertyName => $"Gets the {context.Cpu.Name} interrupt state.",
        _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null)
    };

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