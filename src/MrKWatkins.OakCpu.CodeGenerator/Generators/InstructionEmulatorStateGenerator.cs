using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the explicitly laid-out instruction emulator state fields, facade properties, and execution helpers.
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

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.InstructionEmulator(context);

    protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        var generatorContext = context.GeneratorContext;
        var members = generatorContext.Configuration.Registers.Values.Select(r => ExplicitLayoutBuilder.CreateRegisterField(context, r)).ToList<MemberDeclarationSyntax>();
        members.Add(CreateNoNextSequenceStepField());
        members.Add(CreateConstructor(context));

        var fieldOffset = ExplicitLayoutBuilder.GetRegistersEndOffset(context);

        // Keep the serializable primitive state contiguous after the register block.
        foreach (var dataMember in generatorContext.Configuration.AllDataMembers.Values.Where(m => m != PreDefinedDataMember.CurrentStep && m != PreDefinedDataMember.OpcodeStepTable).OrderByDescending(m => m.Size))
        {
            members.AddRange(CreateDataMember(context, dataMember, fieldOffset));
            fieldOffset += dataMember.Size;
        }

        members.Add(CreateNextSequenceStepField(context, fieldOffset));
        fieldOffset += DataType.U16.Size();

        fieldOffset = ExplicitLayoutBuilder.AlignReferenceFieldOffset(fieldOffset);
        members.Add(CreateObjectProperty(context, Class.Name.InstructionRegisters(context), Property.Name.Registers, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.InstructionFlags(context), Property.Name.Flags, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.InstructionInterrupts(context), Property.Name.Interrupts, fieldOffset));
        fieldOffset += 8;

        members.AddRange(CreateDataMember(context, PreDefinedDataMember.OpcodeStepTable, fieldOffset));
        members.Add(CreateExecuteInstructionMethod(context));
        members.Add(CreateExecuteDecodedInstructionMethod(context));
        members.Add(CreateCompleteInstructionMethod(context));

        return WithXmlDocumentation(
            ClassDeclaration(Class.Name.InstructionEmulator(context))
                .AddModifiers(Public, Sealed, Unsafe, Partial)
                .AddAttributeLists(AttributeList(SingletonSeparatedList(ExplicitLayoutBuilder.CreateStructLayoutAttribute(context))))
                .AddMembers(members.ToArray()),
            $"Represents a {generatorContext.Cpu.Name} emulator that executes one complete instruction at a time.");
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
                        IdentifierName(Field.Name.NextSequenceStep),
                        IdentifierName(Field.Name.NoNextSequenceStep)))
            ],
            (Property.Name.Registers, Class.Name.InstructionRegisters(context)),
            (Property.Name.Flags, Class.Name.InstructionFlags(context)),
            (Property.Name.Interrupts, Class.Name.InstructionInterrupts(context)));

        return WithXmlDocumentation(
            ConstructorDeclaration(Class.Name.InstructionEmulator(context))
                .WithModifiers(TokenList(Public))
                .WithBody(Block(statements)),
            $"Initializes a new {Class.Name.InstructionEmulator(context)} instance.");
    }

    [MustUseReturnValue]
    private static IEnumerable<MemberDeclarationSyntax> CreateDataMember(FileGeneratorContext context, DataMember member, int fieldOffset)
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
                            VariableDeclarator(Field.Name.NoNextSequenceStep)
                                .WithInitializer(
                                    EqualsValueClause(
                                        ParseExpression("ushort.MaxValue"))))))
            .WithModifiers(TokenList(Internal, Token(SyntaxKind.ConstKeyword)));

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateNextSequenceStepField(FileGeneratorContext context, int fieldOffset) =>
        ExplicitLayoutBuilder.CreateOffsetField(context, UShortType, Field.Name.NextSequenceStep, fieldOffset, Internal);

    [Pure]
    private static MethodDeclarationSyntax CreateExecuteInstructionMethod(FileGeneratorContext context)
    {
        const string scheduledSequenceStepVariableName = "scheduledSequenceStep";

        return WithXmlDocumentation(
            MethodDeclaration(IntType, Identifier("ExecuteInstruction"))
                .WithModifiers(TokenList(Public))
                .WithTypeParameterList(InstructionHandlerSyntax.TypeParameters)
                .WithParameterList(ParameterList([InstructionHandlerSyntax.MethodParameter]))
                .WithConstraintClauses(InstructionHandlerSyntax.ConstraintClauses(context.GeneratorContext))
                .WithBody(
                    Block(
                        List<StatementSyntax>(
                        [
                            InitializeVariableStatement(scheduledSequenceStepVariableName, IdentifierName(Field.Name.NextSequenceStep), UShortType),
                            IfStatement(
                                BinaryExpression(
                                    SyntaxKind.NotEqualsExpression,
                                    IdentifierName(scheduledSequenceStepVariableName),
                                    IdentifierName(Field.Name.NoNextSequenceStep)),
                                Block(
                                    List<StatementSyntax>(
                                    [
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(Field.Name.NextSequenceStep),
                                                IdentifierName(Field.Name.NoNextSequenceStep))),
                                        ReturnStatement(
                                            InvocationExpression(IdentifierName(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                    [
                                                        Argument(IdentifierName(scheduledSequenceStepVariableName)),
                                                        InstructionHandlerSyntax.Argument
                                                    ])))
                                    ]))),

                            ReturnStatement(
                                InvocationExpression(IdentifierName(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                        [
                                            Argument(IdentifierName(InstructionEmulatorGenerator.OpcodeReadStep0FieldName)),
                                            InstructionHandlerSyntax.Argument
                                        ])))
                        ]))),
            "Executes one complete instruction or scheduled sequence.",
            parameters: new Dictionary<string, string>
            {
                [InstructionHandlerSyntax.ParameterName] = "Handles the external bus actions required to execute the instruction."
            },
            returns: "The number of T-states executed.");
    }

    [MustUseReturnValue]
    private static MemberDeclarationSyntax CreateExecuteDecodedInstructionMethod(FileGeneratorContext context)
    {
        const string decodedStepParameterName = "decodedStep";
        const string instructionVariableName = "instruction";

        return MethodDeclaration(IntType, Identifier(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
            .AddAttributeLists(CreateAggressiveInliningAttributeList(context.RequiredUsings))
            .WithModifiers(TokenList(Private))
            .WithTypeParameterList(InstructionHandlerSyntax.TypeParameters)
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier(decodedStepParameterName)).WithType(UShortType),
                    InstructionHandlerSyntax.MethodParameter
                ]))
            .WithConstraintClauses(InstructionHandlerSyntax.ConstraintClauses(context.GeneratorContext))
            .WithBody(
                Block(
                    List<StatementSyntax>(
                    [
                        InitializeVariableStatement(
                            instructionVariableName,
                            ElementAccessExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        InstructionHandlerSyntax.DispatchHolderType,
                                        IdentifierName(Field.Name.Instructions)))
                                .WithArgumentList(
                                    BracketedArgumentList(
                                    [
                                        Argument(IdentifierName(decodedStepParameterName))
                                    ]))),
                        ReturnStatement(
                            InvocationExpression(IdentifierName(instructionVariableName))
                                .WithArgumentList(
                                    ArgumentList(
                                    [
                                        Argument(ThisExpression()),
                                        InstructionHandlerSyntax.Argument
                                    ])))
                    ])));
    }

    [MustUseReturnValue]
    private static MemberDeclarationSyntax CreateCompleteInstructionMethod(FileGeneratorContext context)
    {
        const string instructionUpdatesFlagsParameterName = "instructionUpdatesFlags";

        var statements = new List<StatementSyntax>
        {
            InitializeVariableStatement(Parameter.Name.Emulator, ThisExpression())
        };
        statements.AddRange(StatementGenerator.GenerateInstructionCompletionStatements(context, context.GeneratorContext.OnInstructionStepsComplete, instructionUpdatesFlagsParameterName));
        statements.AddRange(StatementGenerator.GenerateInstructionCompletionStatements(context, context.GeneratorContext.OnInstructionComplete, instructionUpdatesFlagsParameterName));
        statements.Add(ReturnStatement(IdentifierName("tStates")));

        return MethodDeclaration(IntType, Identifier(Method.Name.CompleteInstruction))
            .AddAttributeLists(CreateAggressiveInliningAttributeList(context.RequiredUsings))
            .WithModifiers(TokenList(Private))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier(instructionUpdatesFlagsParameterName)).WithType(BoolType),
                    Parameter(Identifier("tStates")).WithType(IntType)
                ]))
            .WithBody(Block(List(statements)));
    }

    [MustUseReturnValue]
    private static PropertyDeclarationSyntax CreateObjectProperty(FileGeneratorContext context, string typeName, string propertyName, int fieldOffset) =>
        WithXmlDocumentation(
            ExplicitLayoutBuilder.CreateGetOnlyPropertyWithFieldOffset(context, typeName, propertyName, fieldOffset),
            ExplicitLayoutBuilder.GetObjectPropertySummary(context, propertyName));
}