using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class SerializationGenerator : TypeGenerator
{
    protected const string SerializeMethodName = "Serialize";
    protected const string DeserializeMethodName = "Deserialize";
    protected const string RestoreMethodName = "Restore";
    protected const string StreamParameterName = "stream";
    protected const string ReaderParameterName = "reader";
    protected const string WriterParameterName = "writer";

    private protected SerializationGenerator()
    {
    }

    [Pure]
    protected sealed override BaseTypeDeclarationSyntax CreateType(GeneratorContext context) =>
        CreateClassDeclaration(context).AddMembers(GenerateSerialize(context), GenerateDeserialize(context), GenerateRestore(context));

    [Pure]
    protected abstract string GetSerializedTypeName(GeneratorContext context);

    [Pure]
    protected virtual ClassDeclarationSyntax CreateClassDeclaration(GeneratorContext context) =>
        ClassDeclaration(GetSerializedTypeName(context)).AddModifiers(Public, Sealed, Unsafe, Partial);

    [Pure]
    protected virtual IEnumerable<DataMember> GetSerializedDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(member => member != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(member => member.Name);

    [Pure]
    protected virtual StatementSyntax GenerateSerializeDataMember(GeneratorContext context, DataMember member) => GenerateWrite(IdentifierName(member.FieldName));

    [Pure]
    protected virtual StatementSyntax GenerateRestoreDataMember(GeneratorContext context, DataMember member) => GenerateRead(member.FieldName, member.Type);

    [Pure]
    protected virtual IEnumerable<StatementSyntax> GenerateAdditionalSerializeStatements(GeneratorContext context) => [];

    [Pure]
    protected virtual IEnumerable<StatementSyntax> GenerateAdditionalRestoreStatements(GeneratorContext context) => [];

    [Pure]
    private MemberDeclarationSyntax GenerateDeserialize(GeneratorContext context)
    {
        const string deserializedVariableName = "deserialized";

        var createEmulator = LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
                .WithVariables(
                [
                    VariableDeclarator(Identifier(deserializedVariableName))
                        .WithInitializer(EqualsValueClause(
                            ObjectCreationExpression(IdentifierName(GetSerializedTypeName(context))).WithArgumentList(ArgumentList())))
                ]));

        var restore = ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(deserializedVariableName), IdentifierName(RestoreMethodName)))
                .WithArgumentList(ArgumentList([Argument(IdentifierName(StreamParameterName))])));

        var returnEmulator = ReturnStatement(IdentifierName(deserializedVariableName));

        return WithXmlDocumentation(
            MethodDeclaration(IdentifierName(GetSerializedTypeName(context)), Identifier(DeserializeMethodName))
                .WithModifiers([Public, Static])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
                ]))
                .WithBody(Block(createEmulator, restore, returnEmulator)),
            $"Deserializes a {context.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [StreamParameterName] = "The stream to read the CPU state from."
            },
            returns: $"The deserialized {context.Cpu.Name} emulator.");
    }

    [Pure]
    private MemberDeclarationSyntax GenerateRestore(GeneratorContext context)
    {
        context.RequiredUsings.Add("System.IO");

        var statements = GenerateUsingBinaryReaderOrWriter<BinaryReader>(context, ReaderParameterName)
            .Concat(GenerateRestoreOpcodeStepTable(context))
            .Concat(GenerateRestoreDataMembers(context))
            .Concat(GenerateAdditionalRestoreStatements(context))
            .Concat(GenerateRestoreRegisters(context));

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(RestoreMethodName))
                .WithModifiers([Public])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
                ]))
                .WithBody(Block(statements)),
            $"Restores this emulator from a serialized {context.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [StreamParameterName] = "The stream to read the CPU state from."
            });
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRestoreOpcodeStepTable(GeneratorContext context)
    {
        var arms = context.Configuration.OpcodeStepTables
            .Select((opcodeStepTables, index) => SwitchExpressionArm(
                ConstantPattern(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(index))),
                IdentifierName(opcodeStepTables.FieldName)))
            .Append(SwitchExpressionArm(
                DiscardPattern(),
                ThrowExpression(ObjectCreationExpression(IdentifierName(nameof(InvalidOperationException)))
                    .WithArgumentList(
                        ArgumentList([Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Unknown opcode step table.")))])))));

        var switchExpression = SwitchExpression(GenerateReadExpression(DataType.U8)).WithArms(SeparatedList(arms));

        yield return ExpressionStatement(
            AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName), switchExpression));
    }

    [Pure]
    private IEnumerable<StatementSyntax> GenerateRestoreDataMembers(GeneratorContext context) =>
        GetSerializedDataMembers(context).Select(member => GenerateRestoreDataMember(context, member));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRestoreRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(register => register.Type == DataType.U8)
            .OrderBy(register => register.FieldOffset)
            .Select(register => GenerateRead(register.FieldName, DataType.U8));

    [Pure]
    private MemberDeclarationSyntax GenerateSerialize(GeneratorContext context)
    {
        context.RequiredUsings.Add("System.IO");

        var statements = GenerateUsingBinaryReaderOrWriter<BinaryWriter>(context, WriterParameterName)
            .Concat(GenerateSerializeOpcodeStepTable(context))
            .Concat(GenerateSerializeDataMembers(context))
            .Concat(GenerateAdditionalSerializeStatements(context))
            .Concat(GenerateSerializeRegisters(context));

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(SerializeMethodName))
                .WithModifiers([Public])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
                ]))
                .WithBody(Block(statements)),
            $"Serializes this emulator's {context.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [StreamParameterName] = "The stream to write the CPU state to."
            });
    }

    [Pure]
    private IEnumerable<StatementSyntax> GenerateSerializeDataMembers(GeneratorContext context) =>
        GetSerializedDataMembers(context).Select(member => GenerateSerializeDataMember(context, member));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSerializeRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(register => register.Type == DataType.U8)
            .OrderBy(register => register.FieldOffset)
            .Select(register => GenerateWrite(IdentifierName(register.FieldName)));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSerializeOpcodeStepTable(GeneratorContext context)
    {
        var opcodeStepTables = context.Configuration.OpcodeStepTables.Reverse().ToArray();

        StatementSyntax? ifStatement = null;
        var index = opcodeStepTables.Length;
        foreach (var opcodeStepTable in opcodeStepTables)
        {
            var condition = BinaryExpression(SyntaxKind.EqualsExpression, IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName), IdentifierName(opcodeStepTable.FieldName));

            var statement = GenerateWrite(CastExpression(ByteType, GenerateNumericLiteralExpression(--index)));

            ifStatement = ifStatement == null
                ? IfStatement(condition, Block(statement))
                : IfStatement(condition, Block(statement), ElseClause(ifStatement));
        }

        yield return ifStatement!;
    }

    [Pure]
    protected static StatementSyntax GenerateWrite(ExpressionSyntax value) =>
        ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(WriterParameterName), IdentifierName(nameof(BinaryWriter.Write))))
                .WithArgumentList(ArgumentList([Argument(value)])));

    [Pure]
    protected static StatementSyntax GenerateRead(string fieldName, DataType type) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(fieldName),
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(ReaderParameterName), IdentifierName(ReadMethodName(type))))));

    [Pure]
    protected static ExpressionSyntax GenerateReadExpression(DataType type) =>
        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(ReaderParameterName), IdentifierName(ReadMethodName(type))));

    [Pure]
    private static string ReadMethodName(DataType type) => type switch
    {
        DataType.U8 => nameof(BinaryReader.ReadByte),
        DataType.U16 => nameof(BinaryReader.ReadUInt16),
        DataType.Bool => nameof(BinaryReader.ReadBoolean),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateUsingBinaryReaderOrWriter<T>(GeneratorContext context, string parameterName)
    {
        context.RequiredUsings.Add(typeof(T));
        context.RequiredUsings.Add(typeof(Encoding));

        yield return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                    .WithVariables(
                    [
                        VariableDeclarator(Identifier(parameterName))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(IdentifierName(typeof(T).Name))
                                        .WithArgumentList(
                                            ArgumentList(
                                            [
                                                Argument(IdentifierName(StreamParameterName)),
                                                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(nameof(Encoding)), IdentifierName(nameof(Encoding.UTF8)))),
                                                Argument(LiteralExpression(SyntaxKind.TrueLiteralExpression))
                                            ]))))
                    ]))
            .WithUsingKeyword(Token(SyntaxKind.UsingKeyword));
    }
}