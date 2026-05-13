using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class InstructionEmulatorGenerator : TypeGenerator
{
    internal const string ExecuteDecodedInstructionMethodName = "ExecuteDecodedInstruction";
    internal const string OpcodeReadStep0FieldName = "OpcodeReadStep0";
    public static readonly InstructionEmulatorGenerator Instance = new();

    private InstructionEmulatorGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => $"{Class.Name.InstructionEmulator(context)}.instructions";

    protected override BaseTypeDeclarationSyntax CreateType(FileGeneratorContext context) =>
        PopulateClass(
            context,
            ClassDeclaration(Class.Name.InstructionEmulator(context)).AddModifiers(Public, Sealed, Unsafe, Partial));

    [Pure]
    private static ClassDeclarationSyntax PopulateClass(FileGeneratorContext context, ClassDeclarationSyntax classDeclaration)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateDispatchConstants(context),
            CreateErrorMethod(context)
        };

        members.AddRange(GetDispatchableSequences(context)
            .Select(sequence => (Sequence: sequence, Steps: GetRegularSteps(sequence)))
            .Select(x => CreateInstructionMethod(context, x.Sequence, x.Steps)));

        return classDeclaration.AddMembers(members.ToArray());
    }

    [Pure]
    internal static IEnumerable<StepSequence> GetDispatchableSequences(GeneratorContext context) => context.InstructionEmulatorSequences;

    [Pure]
    internal static string GetInstructionMethodName(GeneratorContext context, StepSequence sequence)
    {
        var baseName = GetInstructionMethodBaseName(sequence);
        var collisions = GetDispatchableSequences(context)
            .Where(other => string.Equals(GetInstructionMethodBaseName(other), baseName, StringComparison.Ordinal))
            .ToList();
        if (collisions.Count == 1)
        {
            return baseName;
        }

        var canonical = collisions
            .OrderBy(GetInstructionMethodCollisionRank)
            .ThenBy(context.GetInstructionEmulatorSequenceIndex)
            .First();

        return ReferenceEquals(canonical, sequence)
            ? baseName
            : $"{baseName}_{GetInstructionMethodEncodingSuffix(sequence)}";
    }

    [Pure]
    private static int GetInstructionMethodCollisionRank(StepSequence sequence) =>
        sequence switch
        {
            Instruction { Prefix: null, OpcodeTable: null } => 0,
            Instruction { Prefix: not null, OpcodeTable: null } => 1,
            Instruction { Prefix: null, OpcodeTable: not null } => 2,
            Instruction { Prefix: not null, OpcodeTable: not null } => 3,
            PrefixJump => 4,
            _ => 5
        };

    [Pure]
    private static string GetInstructionMethodBaseName(StepSequence sequence) =>
        sequence switch
        {
            Instruction instruction => SanitizeIdentifier(instruction.Mnemonic),
            PrefixJump prefixJump => $"Prefix_{prefixJump.Prefix:X2}",
            NamedStepSequence namedSequence => SanitizeIdentifier(GetNamedSequenceDisplayName(namedSequence.Name ?? namedSequence.FirstStep.Name)),
            _ => SanitizeIdentifier(sequence.Name ?? sequence.FirstStep.Name)
        };

    [Pure]
    private static string GetInstructionMethodEncodingSuffix(StepSequence sequence) =>
        sequence switch
        {
            Instruction { OpcodeTable: { } opcodeTable, Prefix: { } prefix, Opcode: var opcode } => $"{SanitizeIdentifier(opcodeTable)}_{prefix:X2}_{opcode:X2}",
            Instruction { Prefix: { } prefix, Opcode: var opcode } => $"{prefix:X2}_{opcode:X2}",
            Instruction { OpcodeTable: { } opcodeTable, Opcode: var opcode } => $"{SanitizeIdentifier(opcodeTable)}_{opcode:X2}",
            Instruction { Opcode: var opcode } => $"{opcode:X2}",
            PrefixJump { Prefix: var prefix } => $"{prefix:X2}",
            NamedStepSequence namedSequence => SanitizeIdentifier(GetNamedSequenceDisplayName(namedSequence.Name ?? namedSequence.FirstStep.Name)),
            _ => SanitizeIdentifier(sequence.Name ?? sequence.FirstStep.Name)
        };

    [Pure]
    private static string GetInstructionMethodComment(StepSequence sequence) =>
        sequence switch
        {
            Instruction instruction => instruction.Mnemonic,
            PrefixJump prefixJump => $"Read opcode after prefix 0x{prefixJump.Prefix:X2}",
            NamedStepSequence namedSequence => GetNamedSequenceDisplayName(namedSequence.Name ?? namedSequence.FirstStep.Name),
            _ => sequence.Name ?? sequence.FirstStep.Name
        };

    [Pure]
    private static string GetNamedSequenceDisplayName(string name) =>
        name switch
        {
            "opcode_read" => "Opcode read",
            "halted" => "Halt cycle",
            "halted_cycle" => "Halt cycle",
            _ when name.StartsWith("interrupt_mode_", StringComparison.Ordinal) &&
                   byte.TryParse(name["interrupt_mode_".Length..], out var mode)
                => $"Interrupt Mode {mode}",
            _ => name.Replace('_', ' ')
        };

    [Pure]
    private static string SanitizeIdentifier(string value)
    {
        var builder = new StringBuilder();
        var previousUnderscore = false;

        foreach (var c in value)
        {
            var replacement = c switch
            {
                '\'' => "_shadow",
                '+' => "_plus_",
                ' ' or ',' or '-' or '/' or '.' => "_",
                '(' or ')' => "",
                _ when char.IsLetterOrDigit(c) => c.ToString(),
                _ => "_"
            };

            foreach (var replacementCharacter in replacement)
            {
                if (replacementCharacter == '_')
                {
                    if (previousUnderscore)
                    {
                        continue;
                    }

                    previousUnderscore = true;
                }
                else
                {
                    previousUnderscore = false;
                }

                builder.Append(replacementCharacter);
            }
        }

        var result = builder.ToString().Trim('_');
        if (result.Length == 0)
        {
            return "Instruction";
        }

        return char.IsDigit(result[0]) ? $"_{result}" : result;
    }

    [Pure]
    private static IReadOnlyList<Step> GetRegularSteps(StepSequence sequence) => sequence.Steps.Where(step => !step.ExecutesAsOverlapOnly).ToList();

    [Pure]
    private static FieldDeclarationSyntax CreateDispatchConstants(GeneratorContext context)
    {
        var opcodeReadStart = context.GetInstructionEmulatorSequenceIndex(context.OpcodeRead);

        return FieldDeclaration(
                VariableDeclaration(UShortType)
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(OpcodeReadStep0FieldName)
                                .WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(opcodeReadStart)))))))
            .WithModifiers([Private, Token(SyntaxKind.ConstKeyword)]);
    }

    [MustUseReturnValue]
    private static MethodDeclarationSyntax CreateErrorMethod(FileGeneratorContext context)
    {
        context.RequiredUsings.Add(typeof(NotSupportedException).Namespace!);

        return MethodDeclaration(IntType, Identifier(Method.Name.Error))
            .WithModifiers([Private, Static])
            .WithParameterList(
                ParameterList(
                [
                    Parameter.Syntax.InstructionEmulator(context),
                    Parameter.Syntax.InstructionActionCallback()
                ]))
            .WithBody(
                Block(
                    ThrowStatement(
                        ObjectCreationExpression(IdentifierName(nameof(NotSupportedException)))
                            .WithArgumentList(
                                ArgumentList(
                                [
                                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Opcode not supported")))
                                ])))));
    }

    [Pure]
    private static MemberDeclarationSyntax CreateInstructionMethod(FileGeneratorContext context, StepSequence sequence, IReadOnlyList<Step> steps)
    {
        if (sequence is PrefixJump prefixJump)
        {
            return CreatePrefixJumpMethod(context, prefixJump, steps);
        }

        var plan = InstructionMethodPlanner.CreatePlan(
            context,
            sequence,
            steps,
            GetInstructionMethodName(context, sequence),
            GetInstructionMethodComment(sequence));

        return InstructionMethodEmitter.CreateMethod(context, plan);
    }

    [Pure]
    private static MemberDeclarationSyntax CreatePrefixJumpMethod(FileGeneratorContext context, PrefixJump sequence, IReadOnlyList<Step> steps)
    {
        var comments = new[] { Comment($"// {GetInstructionMethodComment(sequence)}") };
        var statements = new List<StatementSyntax>();

        foreach (var step in steps)
        {
            var stepStatements = StatementGenerator.GenerateInstructionStatements(context, step, null, null, 0, 0).ToList();
            if (stepStatements.Count != 0)
            {
                statements.AddRange(stepStatements);
            }
        }

        statements.Add(
            ReturnStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(Parameter.Name.Emulator),
                            IdentifierName(ExecuteDecodedInstructionMethodName)))
                    .WithArgumentList(
                        ArgumentList(
                        [
                            Argument(IdentifierName(OpcodeReadStep0FieldName)),
                            Argument(IdentifierName(Parameter.Name.InstructionActionCallback))
                        ]))));

        return MethodDeclaration(IntType, Identifier(GetInstructionMethodName(context, sequence)))
            .WithModifiers([Private, Static])
            .WithParameterList(
                ParameterList(
                [
                    Parameter.Syntax.InstructionEmulator(context),
                    Parameter.Syntax.InstructionActionCallback()
                ]))
            .WithLeadingTrivia(comments)
            .WithBody(Block(statements));
    }

}