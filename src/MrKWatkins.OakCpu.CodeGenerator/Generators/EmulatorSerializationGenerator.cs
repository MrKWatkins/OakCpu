using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorSerializationGenerator : EmulatorClassGenerator
{
    private const string SerializeMethodName = "Serialize";
    private const string DeserializeMethodName = "Deserialize";
    private const string RestoreMethodName = "Restore";
    private const string StreamParameterName = "stream";
    private const string ReaderParameterName = "reader";
    private const string WriterParameterName = "writer";

    public static readonly EmulatorSerializationGenerator Instance = new();

    private EmulatorSerializationGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{GetEmulatorClassName(context)}.serialization";

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(GenerateSerialize(context), GenerateDeserialize(context), GenerateRestore(context));

    [Pure]
    private static MemberDeclarationSyntax GenerateDeserialize(GeneratorContext context)
    {
        const string deserializedVariableName = "deserialized";

        var createEmulator = LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
                .WithVariables(
                [
                    VariableDeclarator(Identifier(deserializedVariableName))
                        .WithInitializer(EqualsValueClause(
                            ObjectCreationExpression(IdentifierName(GetEmulatorClassName(context))).WithArgumentList(ArgumentList())))
                ]));

        var restore = ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(deserializedVariableName), IdentifierName(RestoreMethodName)))
                .WithArgumentList(ArgumentList([Argument(IdentifierName(StreamParameterName))])));

        var returnEmulator = ReturnStatement(IdentifierName(deserializedVariableName));

        return MethodDeclaration(IdentifierName(GetEmulatorClassName(context)), Identifier(DeserializeMethodName))
            .WithModifiers([Public, Static])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
            ]))
            .WithBody(Block(createEmulator, restore, returnEmulator));
    }
    [Pure]
    private static MemberDeclarationSyntax GenerateRestore(GeneratorContext context)
    {
        context.RequiredUsings.Add("System.IO");

        var statements = GenerateUsingBinaryReaderOrWriter<BinaryReader>(context, ReaderParameterName)
            .Concat(GenerateRestoreOpcodeStepTable(context))
            .Concat(GenerateRestoreDataMembers(context))
            .Concat(GenerateRestoreRegisters(context));

        return MethodDeclaration(VoidType, Identifier(RestoreMethodName))
            .WithModifiers([Public])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
            ]))
            .WithBody(Block(statements));
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
    private static IEnumerable<StatementSyntax> GenerateRestoreDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(m => m.Name)
            .Select(m => GenerateRead(m.FieldName, m.Type));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateRestoreRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(r => r.Type == DataType.U8)
            .OrderBy(r => r.FieldOffset)
            .Select(r => GenerateRead(r.FieldName, DataType.U8));

    [Pure]
    private static MemberDeclarationSyntax GenerateSerialize(GeneratorContext context)
    {
        context.RequiredUsings.Add("System.IO");

        var statements = GenerateUsingBinaryReaderOrWriter<BinaryWriter>(context, WriterParameterName)
            .Concat(GenerateSerializeOpcodeStepTable(context))
            .Concat(GenerateSerializeDataMembers(context))
            .Concat(GenerateSerializeRegisters(context));

        return MethodDeclaration(VoidType, Identifier(SerializeMethodName))
            .WithModifiers([Public])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
            ]))
            .WithBody(Block(statements));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSerializeDataMembers(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(m => m != PreDefinedDataMember.OpcodeStepTable)
            .OrderBy(m => m.Name)
            .Select(m => GenerateWrite(IdentifierName(m.FieldName)));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSerializeRegisters(GeneratorContext context) =>
        context.Configuration.Registers.Values
            .Where(r => r.Type == DataType.U8)
            .OrderBy(r => r.FieldOffset)
            .Select(r => GenerateWrite(IdentifierName(r.FieldName)));

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateSerializeOpcodeStepTable(GeneratorContext context)
    {
        var opcodeStepTables = context.Configuration.OpcodeStepTables.Reverse().ToArray();

        StatementSyntax? ifStatement = null;
        var index = opcodeStepTables.Length;
        foreach (var opcodeStepTable in opcodeStepTables)
        {
            var condition = BinaryExpression(SyntaxKind.EqualsExpression, IdentifierName(PreDefinedDataMember.OpcodeStepTable.FieldName), IdentifierName(opcodeStepTable.FieldName));

            var statement = GenerateWrite(CastExpression(PredefinedType(Token(SyntaxKind.ByteKeyword)), GenerateNumericLiteralExpression(--index)));

            ifStatement = ifStatement == null
                ? IfStatement(condition, Block(statement))
                : IfStatement(condition, Block(statement), ElseClause(ifStatement));
        }

        yield return ifStatement!;
    }

    [Pure]
    private static StatementSyntax GenerateWrite(ExpressionSyntax value) =>
        ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(WriterParameterName), IdentifierName(nameof(BinaryWriter.Write))))
                .WithArgumentList(ArgumentList([Argument(value)])));

    [Pure]
    private static StatementSyntax GenerateRead(string fieldName, DataType type) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(fieldName),
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(ReaderParameterName), IdentifierName(ReadMethodName(type))))));

    [Pure]
    private static ExpressionSyntax GenerateReadExpression(DataType type) =>
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
        context.RequiredUsings.Add(typeof(T).Namespace!);
        context.RequiredUsings.Add(typeof(Encoding).Namespace!);

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