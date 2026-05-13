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

        var fieldOffset = ExplicitLayoutBuilder.GetObjectPropertiesFieldOffset(context);
        members.Add(CreateObjectProperty(context, Class.Name.Registers(context), Property.Name.Registers, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.Flags(context), Property.Name.Flags, fieldOffset));
        fieldOffset += 8;

        members.Add(CreateObjectProperty(context, Class.Name.Interrupts(context), Property.Name.Interrupts, fieldOffset));
        fieldOffset += 8;

        foreach (var dataMember in generatorContext.Configuration.AllDataMembers.Values.Where(m => m != PreDefinedDataMember.CurrentStep).OrderByDescending(m => m.Size))
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

    [Pure]
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

    [Pure]
    private static FieldDeclarationSyntax CreateNextSequenceStepField(FileGeneratorContext context, int fieldOffset) =>
        ExplicitLayoutBuilder.CreateOffsetField(context, UShortType, Field.Name.NextSequenceStep, fieldOffset, Internal);

    [Pure]
    private static MethodDeclarationSyntax CreateExecuteInstructionMethod()
    {
        const string scheduledSequenceStepVariableName = "scheduledSequenceStep";

        return WithXmlDocumentation(
            MethodDeclaration(IntType, Identifier("ExecuteInstruction"))
                .WithModifiers(TokenList(Public))
                .WithParameterList(ParameterList([Parameter.Syntax.InstructionActionCallback()]))
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
                                            Argument(IdentifierName(Parameter.Name.InstructionActionCallback))
                                        ]))),

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
                                                        Argument(IdentifierName(Parameter.Name.InstructionActionCallback))
                                                    ])))
                                    ]))),

                            ReturnStatement(
                                InvocationExpression(IdentifierName(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                        [
                                            Argument(IdentifierName(InstructionEmulatorGenerator.OpcodeReadStep0FieldName)),
                                            Argument(IdentifierName(Parameter.Name.InstructionActionCallback))
                                        ])))
                        ]))),
            "Executes one complete instruction or scheduled sequence.",
            parameters: new Dictionary<string, string>
            {
                [Parameter.Name.InstructionActionCallback] = "Called whenever the emulator requires an external bus action."
            },
            returns: "The number of T-states executed.");
    }

    [Pure]
    private static MemberDeclarationSyntax CreateExecuteDecodedInstructionMethod(FileGeneratorContext context)
    {
        const string decodedStepParameterName = "decodedStep";
        const string instructionVariableName = "instruction";

        return MethodDeclaration(IntType, Identifier(InstructionEmulatorGenerator.ExecuteDecodedInstructionMethodName))
            .AddAttributeLists(AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]))
            .WithModifiers(TokenList(Private))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier(decodedStepParameterName)).WithType(UShortType),
                    Parameter.Syntax.InstructionActionCallback()
                ]))
            .WithBody(
                Block(
                    List<StatementSyntax>(
                    [
                        InitializeVariableStatement(
                            instructionVariableName,
                            ElementAccessExpression(IdentifierName(Field.Name.Instructions))
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
                                        Argument(IdentifierName(Parameter.Name.InstructionActionCallback))
                                    ])))
                    ])));
    }

    [Pure]
    private static MemberDeclarationSyntax CreateCompleteInstructionMethod(FileGeneratorContext context)
    {
        const string instructionUpdatesFlagsParameterName = "instructionUpdatesFlags";

        var statements = new List<StatementSyntax>
        {
            InitializeVariableStatement(Parameter.Name.Emulator, ThisExpression())
        };
        statements.AddRange(StatementGenerator.GenerateInstructionCompletionStatements(context, context.GeneratorContext.OnInstructionComplete, instructionUpdatesFlagsParameterName));
        statements.Add(ReturnStatement(IdentifierName("tStates")));

        return MethodDeclaration(IntType, Identifier(Method.Name.CompleteInstruction))
            .AddAttributeLists(AttributeList([CreateMethodImplAttribute(context.RequiredUsings, System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]))
            .WithModifiers(TokenList(Private))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier(instructionUpdatesFlagsParameterName)).WithType(BoolType),
                    Parameter(Identifier("tStates")).WithType(IntType)
                ]))
            .WithBody(Block(List(statements)));
    }

    [Pure]
    private static PropertyDeclarationSyntax CreateObjectProperty(FileGeneratorContext context, string typeName, string propertyName, int fieldOffset) =>
        WithXmlDocumentation(
            ExplicitLayoutBuilder.CreateGetOnlyPropertyWithFieldOffset(context, typeName, propertyName, fieldOffset),
            ExplicitLayoutBuilder.GetObjectPropertySummary(context, propertyName));
}