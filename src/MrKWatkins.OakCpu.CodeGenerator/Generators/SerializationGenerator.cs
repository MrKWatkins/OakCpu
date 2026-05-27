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

    private const string DestinationParameterName = "destination";
    private const string SourceParameterName = "source";
    private const string SerializedSizeFieldName = "SerializedSize";

    private protected SerializationGenerator()
    {
    }

    [Pure]
    protected sealed override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context)
    {
        var layout = CreateLayout(context.GeneratorContext);

        return CreateClassDeclaration(context.GeneratorContext).AddMembers(
            GenerateSerializedSize(layout),
            GenerateSerializeSpan(context, layout),
            GenerateSerializeStream(context),
            GenerateDeserializeSpan(context),
            GenerateDeserializeStream(context),
            GenerateRestoreSpan(context, layout),
            GenerateRestoreStream(context));
    }

    [Pure]
    protected abstract string GetSerializedTypeName(GeneratorContext context);

    [Pure]
    protected virtual bool SerializesOverlapPipeline => false;

    [Pure]
    protected virtual ClassDeclarationSyntax CreateClassDeclaration(GeneratorContext context) =>
        ClassDeclaration(GetSerializedTypeName(context)).AddModifiers(Public, Sealed, Unsafe, Partial);

    [Pure]
    protected virtual IEnumerable<DataMember> GetSerializedDataFields(GeneratorContext context) =>
        context.Configuration.AllDataMembers.Values
            .Where(member => member != PreDefinedDataMember.OpcodeStepTable)
            .OrderByDescending(member => member.Size);

    [Pure]
    protected virtual IEnumerable<SerializedField> GetAdditionalSerializedFields(GeneratorContext context, int nextFieldOffset) => [];

    [Pure]
    private SerializationLayout CreateLayout(GeneratorContext context)
    {
        var rawBlocks = CreateRawBlocks(GetSerializedRawFields(context)).ToArray();
        var serializedSize = 1 + (SerializesOverlapPipeline ? 2 : 0) + rawBlocks.Sum(block => block.Size);
        return new SerializationLayout(rawBlocks, serializedSize);
    }

    [Pure]
    private IEnumerable<SerializedField> GetSerializedRawFields(GeneratorContext context)
    {
        foreach (var register in context.Configuration.Registers.Values.Where(register => register.Type == DataType.U8).OrderBy(register => register.FieldOffset))
        {
            yield return new SerializedField(register.FieldName, register.Type, register.FieldOffset, register.Type.Size());
        }

        var fieldOffset = ExplicitLayoutBuilder.GetRegistersEndOffset(context);

        foreach (var member in GetSerializedDataFields(context))
        {
            yield return new SerializedField(member.FieldName, member.Type, fieldOffset, member.Size);
            fieldOffset += member.Size;
        }

        foreach (var field in GetAdditionalSerializedFields(context, fieldOffset))
        {
            yield return field;
        }
    }

    [Pure]
    private static IReadOnlyList<SerializedBlock> CreateRawBlocks(IEnumerable<SerializedField> fields)
    {
        var blocks = new List<SerializedBlock>();

        SerializedField? firstField = null;
        var nextFieldOffset = 0;
        var size = 0;
        foreach (var field in fields.OrderBy(field => field.FieldOffset))
        {
            if (firstField == null)
            {
                firstField = field;
                nextFieldOffset = field.FieldOffset + field.Size;
                size = field.Size;
                continue;
            }

            if (field.FieldOffset == nextFieldOffset)
            {
                nextFieldOffset += field.Size;
                size += field.Size;
                continue;
            }

            blocks.Add(new SerializedBlock(firstField.FieldName, firstField.Type, size));
            firstField = field;
            nextFieldOffset = field.FieldOffset + field.Size;
            size = field.Size;
        }

        if (firstField != null)
        {
            blocks.Add(new SerializedBlock(firstField.FieldName, firstField.Type, size));
        }

        return blocks;
    }

    [Pure]
    private static MemberDeclarationSyntax GenerateSerializedSize(SerializationLayout layout) =>
        WithXmlDocumentation(
            FieldDeclaration(
                    VariableDeclaration(IntType)
                        .WithVariables(
                        [
                            VariableDeclarator(Identifier(SerializedSizeFieldName))
                                .WithInitializer(EqualsValueClause(GenerateNumericLiteralExpression(layout.SerializedSize)))
                        ]))
                .WithModifiers(TokenList(Public, Token(SyntaxKind.ConstKeyword))),
            "The number of bytes required to serialize the emulator state.");

    [Pure]
    private MemberDeclarationSyntax GenerateDeserializeSpan(GeneratorContext context)
    {
        const string deserializedVariableName = "deserialized";

        var createEmulator = InitializeVariableStatement(
            deserializedVariableName,
            ObjectCreationExpression(IdentifierName(GetSerializedTypeName(context))).WithArgumentList(ArgumentList()));

        var restore = ExpressionStatement(
            InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(deserializedVariableName), IdentifierName(RestoreMethodName)))
                .WithArgumentList(ArgumentList([Argument(IdentifierName(SourceParameterName))])));

        var returnEmulator = ReturnStatement(IdentifierName(deserializedVariableName));

        return WithXmlDocumentation(
            MethodDeclaration(IdentifierName(GetSerializedTypeName(context)), Identifier(DeserializeMethodName))
                .WithModifiers([Public, Static])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(SourceParameterName))
                        .WithType(GenericName(Identifier("ReadOnlySpan")).WithTypeArgumentList(TypeArgumentList([ByteType])))
                ]))
                .WithBody(Block(createEmulator, restore, returnEmulator)),
            $"Deserializes a {context.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [SourceParameterName] = "The serialized CPU state to read from."
            },
            returns: $"The deserialized {context.Cpu.Name} emulator.");
    }

    [MustUseReturnValue]
    private MemberDeclarationSyntax GenerateDeserializeStream(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(Stream));

        var buffer = ParseStatement($"Span<byte> buffer = stackalloc byte[{SerializedSizeFieldName}];");
        var read = ParseStatement($"{StreamParameterName}.ReadExactly(buffer);");
        var returnEmulator = ParseStatement($"return {DeserializeMethodName}(buffer);");

        return WithXmlDocumentation(
            MethodDeclaration(IdentifierName(GetSerializedTypeName(context.GeneratorContext)), Identifier(DeserializeMethodName))
                .WithModifiers([Public, Static])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
                ]))
                .WithBody(Block(buffer, read, returnEmulator)),
            $"Deserializes a {context.GeneratorContext.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [StreamParameterName] = "The stream to read the CPU state from."
            },
            returns: $"The deserialized {context.GeneratorContext.Cpu.Name} emulator.");
    }

    [MustUseReturnValue]
    private MemberDeclarationSyntax GenerateRestoreSpan(FileGeneratorContext context, SerializationLayout layout)
    {
        var statements = new List<StatementSyntax>
        {
            GenerateSpanLengthGuard(SourceParameterName, "source"),
            GenerateRestoreOpcodeStepTable(context.GeneratorContext)
        };

        var serializedOffset = 1;
        if (SerializesOverlapPipeline)
        {
            statements.Add(ParseStatement($"RestoreOverlapPipeline((ushort)({SourceParameterName}[1] | {SourceParameterName}[2] << 8));"));
            serializedOffset += 2;
        }

        foreach (var (block, index) in layout.RawBlocks.Select((block, index) => (block, index)))
        {
            statements.Add(GenerateRestoreRawBlock(block, index, serializedOffset));
            serializedOffset += block.Size;
        }

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(RestoreMethodName))
                .WithModifiers([Public])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(SourceParameterName))
                        .WithType(GenericName(Identifier("ReadOnlySpan")).WithTypeArgumentList(TypeArgumentList([ByteType])))
                ]))
                .WithBody(Block(statements)),
            $"Restores this emulator from a serialized {context.GeneratorContext.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [SourceParameterName] = "The serialized CPU state to read from."
            });
    }

    [MustUseReturnValue]
    private MemberDeclarationSyntax GenerateRestoreStream(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(Stream));

        var buffer = ParseStatement($"Span<byte> buffer = stackalloc byte[{SerializedSizeFieldName}];");
        var read = ParseStatement($"{StreamParameterName}.ReadExactly(buffer);");
        var restore = ParseStatement($"{RestoreMethodName}(buffer);");

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(RestoreMethodName))
                .WithModifiers([Public])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
                ]))
                .WithBody(Block(buffer, read, restore)),
            $"Restores this emulator from a serialized {context.GeneratorContext.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [StreamParameterName] = "The stream to read the CPU state from."
            });
    }

    [MustUseReturnValue]
    private MemberDeclarationSyntax GenerateSerializeSpan(FileGeneratorContext context, SerializationLayout layout)
    {
        var statements = new List<StatementSyntax>
        {
            GenerateSpanLengthGuard(DestinationParameterName, "destination"),
            GenerateSerializeOpcodeStepTable(context.GeneratorContext)
        };

        var serializedOffset = 1;
        if (SerializesOverlapPipeline)
        {
            statements.AddRange(
            [
                ParseStatement("ushort overlapIndex = SerializeOverlapPipeline();"),
                ParseStatement($"{DestinationParameterName}[1] = (byte)overlapIndex;"),
                ParseStatement($"{DestinationParameterName}[2] = (byte)(overlapIndex >> 8);")
            ]);
            serializedOffset += 2;
        }

        foreach (var (block, index) in layout.RawBlocks.Select((block, index) => (block, index)))
        {
            statements.Add(GenerateSerializeRawBlock(block, index, serializedOffset));
            serializedOffset += block.Size;
        }

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(SerializeMethodName))
                .WithModifiers([Public])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(DestinationParameterName))
                        .WithType(GenericName(Identifier("Span")).WithTypeArgumentList(TypeArgumentList([ByteType])))
                ]))
                .WithBody(Block(statements)),
            $"Serializes this emulator's {context.GeneratorContext.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [DestinationParameterName] = "The destination span to write the CPU state to."
            });
    }

    [MustUseReturnValue]
    private MemberDeclarationSyntax GenerateSerializeStream(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(Stream));

        var buffer = ParseStatement($"Span<byte> buffer = stackalloc byte[{SerializedSizeFieldName}];");
        var serialize = ParseStatement($"{SerializeMethodName}(buffer);");
        var write = ParseStatement($"{StreamParameterName}.Write(buffer);");

        return WithXmlDocumentation(
            MethodDeclaration(VoidType, Identifier(SerializeMethodName))
                .WithModifiers([Public])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier(StreamParameterName)).WithType(IdentifierName(nameof(Stream)))
                ]))
                .WithBody(Block(buffer, serialize, write)),
            $"Serializes this emulator's {context.GeneratorContext.Cpu.Name} CPU state.",
            parameters: new Dictionary<string, string>
            {
                [StreamParameterName] = "The stream to write the CPU state to."
            });
    }

    [Pure]
    private static StatementSyntax GenerateSpanLengthGuard(string parameterName, string parameterDescription) =>
        ParseStatement(
            $$"""
            if ({{parameterName}}.Length < {{SerializedSizeFieldName}})
            {
                throw new ArgumentException("The {{parameterDescription}} span is too small.", nameof({{parameterName}}));
            }
            """);

    [Pure]
    private static StatementSyntax GenerateSerializeRawBlock(SerializedBlock block, int index, int serializedOffset) =>
        ParseStatement(
            $$"""
            fixed ({{block.Type.TypeSyntax()}}* block{{index}} = &{{block.FieldName}})
            {
                new ReadOnlySpan<byte>((byte*)block{{index}}, {{block.Size}}).CopyTo({{DestinationParameterName}}.Slice({{serializedOffset}}, {{block.Size}}));
            }
            """);

    [Pure]
    private static StatementSyntax GenerateRestoreRawBlock(SerializedBlock block, int index, int serializedOffset) =>
        ParseStatement(
            $$"""
            fixed ({{block.Type.TypeSyntax()}}* block{{index}} = &{{block.FieldName}})
            {
                {{SourceParameterName}}.Slice({{serializedOffset}}, {{block.Size}}).CopyTo(new Span<byte>((byte*)block{{index}}, {{block.Size}}));
            }
            """);

    [Pure]
    private static StatementSyntax GenerateRestoreOpcodeStepTable(GeneratorContext context)
    {
        var arms = context.Configuration.OpcodeStepTables
            .Select((opcodeStepTables, index) => $"{index} => {opcodeStepTables.FieldName},")
            .Append("_ => throw new InvalidOperationException(\"Unknown opcode step table.\")");

        return ParseStatement(
            $$"""
            opcodeStepTable = {{SourceParameterName}}[0] switch
            {
                {{string.Join(Environment.NewLine + "    ", arms)}}
            };
            """);
    }

    [Pure]
    private static StatementSyntax GenerateSerializeOpcodeStepTable(GeneratorContext context)
    {
        var builder = new StringBuilder();
        var opcodeStepTables = context.Configuration.OpcodeStepTables.ToArray();
        for (var index = 0; index < opcodeStepTables.Length; index++)
        {
            var keyword = index == 0 ? "if" : "else if";
            builder.AppendLine($"{keyword} (opcodeStepTable == {opcodeStepTables[index].FieldName})");
            builder.AppendLine("{");
            builder.AppendLine($"    {DestinationParameterName}[0] = (byte){index};");
            builder.AppendLine("}");
        }

        builder.AppendLine("else");
        builder.AppendLine("{");
        builder.AppendLine("    throw new InvalidOperationException(\"Unknown opcode step table.\");");
        builder.AppendLine("}");

        return ParseStatement(builder.ToString());
    }

    protected sealed record SerializedField(string FieldName, DataType Type, int FieldOffset, int Size);

    private sealed record SerializedBlock(string FieldName, DataType Type, int Size);

    private sealed record SerializationLayout(IReadOnlyList<SerializedBlock> RawBlocks, int SerializedSize);
}