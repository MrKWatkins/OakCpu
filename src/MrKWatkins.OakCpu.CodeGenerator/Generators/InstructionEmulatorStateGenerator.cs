using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratedNames;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.GeneratorSymbols;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the explicitly laid out instruction-emulator state fields, facade properties, and execution helpers.
/// </summary>
public sealed class InstructionEmulatorStateGenerator : TypeGenerator
{
    /// <summary>
    /// The singleton instance of the generator.
    /// </summary>
    public static readonly InstructionEmulatorStateGenerator Instance = new();

    private InstructionEmulatorStateGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => GetInstructionEmulatorClassName(context);

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = context.Configuration.Registers.Values.Select(r => ExplicitLayoutBuilder.CreateRegisterField(context, r)).ToList<MemberDeclarationSyntax>();
        members.Add(CreateNoNextSequenceStepField());
        members.Add(CreateConstructor(context));

        var fieldOffset = ExplicitLayoutBuilder.GetObjectPropertiesFieldOffset(context);
        members.Add(CreateObjectProperty(context, GetRegistersClassName(context), RegistersPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, GetFlagsClassName(context), FlagsPropertyName, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, GetInterruptsClassName(context), InterruptsPropertyName, fieldOffset));
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
                .AddAttributeLists(AttributeList(SingletonSeparatedList(ExplicitLayoutBuilder.CreateStructLayoutAttribute(context))))
                .AddMembers(members.ToArray()),
            $"Represents a {context.Cpu.Name} emulator that executes one complete instruction at a time.");
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context)
    {
        var statements = ExplicitLayoutBuilder.CreateConstructorStatements(
            context,
            [
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(NextSequenceStepFieldName),
                        IdentifierName(NoNextSequenceStepFieldName)))
            ],
            (RegistersPropertyName, GetInstructionRegistersClassName(context)),
            (FlagsPropertyName, GetInstructionFlagsClassName(context)),
            (InterruptsPropertyName, GetInstructionInterruptsClassName(context)));

        return WithXmlDocumentation(
            ConstructorDeclaration(GetInstructionEmulatorClassName(context))
                .WithModifiers(TokenList(Public))
                .WithBody(Block(statements)),
            $"Initializes a new {GetInstructionEmulatorClassName(context)} instance.");
    }

    [Pure]
    private static IEnumerable<MemberDeclarationSyntax> CreateDataMember(GeneratorContext context, DataMember member, int fieldOffset)
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
        ExplicitLayoutBuilder.CreateOffsetField(context, UShortType, NextSequenceStepFieldName, fieldOffset, Internal);

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
    private static PropertyDeclarationSyntax CreateObjectProperty(GeneratorContext context, string typeName, string propertyName, int fieldOffset) =>
        WithXmlDocumentation(
            ExplicitLayoutBuilder.CreateGetOnlyPropertyWithFieldOffset(context, typeName, propertyName, fieldOffset),
            ExplicitLayoutBuilder.GetObjectPropertySummary(context, propertyName));
}