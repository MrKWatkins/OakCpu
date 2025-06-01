using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class FlagsGenerator : Generator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateFlagsStatements(GeneratorInput input, Step step)
    {
        var instruction = step.Instruction ?? throw new InvalidOperationException("Cannot use flags() outside of an instruction.");
        var flagsVariableName = $"flags{step.Index}";

        bool initialized;
        var constants = GenerateConstantStatement(input, instruction, flagsVariableName);
        if (constants != null)
        {
            yield return constants;
            initialized = true;
        }
        else
        {
            initialized = false;
        }

        foreach (var statement in GenerateCopyFromStatements(input, instruction, flagsVariableName, initialized))
        {
            yield return statement;
        }

        foreach (var statement in GenerateCallExpressionStatements(input, instruction, flagsVariableName))
        {
            yield return statement;
        }

        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(input.FlagsRegister.FieldName),
                CastExpression(
                    input.FlagsRegister.TypeSyntax, IdentifierName(flagsVariableName))));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCallExpressionStatements(GeneratorInput input, Instruction instruction, string flagsVariableName)
    {
        foreach (var (flag, call) in EnumerateCallExpressions(input, instruction))
        {
            if (call.Function == PreDefinedFunction.IsZero)
            {
                yield return GenerateIsZeroStatement(flagsVariableName, flag, call);
            }
            else if (call.Function is UserDefinedFunction userDefinedFunction)
            {
                yield return GenerateUserDefinedFunctionStatement(flagsVariableName, flag, call, userDefinedFunction);
            }
        }
    }

    [Pure]
    private static StatementSyntax GenerateUserDefinedFunctionStatement(string flagsVariableName, Flag flag, Call call, UserDefinedFunction userDefinedFunction)
    {
        if (!userDefinedFunction.IsBooleanLike)
        {
            throw new InvalidOperationException("Flags can only be set from Boolean-like functions.");
        }

        var value = BinaryExpression(SyntaxKind.LeftShiftExpression, ExpressionGenerator.GenerateExpressionSyntax(call), GenerateNumericLiteralExpression(flag.Index));

        return CreateFlagsOrAssignment(flagsVariableName, $"// Set {flag} from {call}.", value);
    }

    [Pure]
    private static StatementSyntax GenerateIsZeroStatement(string flagsVariableName, Flag flag, Call call)
    {
        var condition = BinaryExpression(SyntaxKind.EqualsExpression, ExpressionGenerator.GenerateExpressionSyntax(call.Arguments[0]), GenerateNumericLiteralExpression(0));

        var bitMask = BuildBitMask(flag);

        return IfStatement(condition, Block(CreateFlagsOrAssignment(flagsVariableName, $"// Set {flag} when {call.Arguments[0]} is zero.", GenerateBinaryLiteralExpression(bitMask))));
    }

    [Pure]
    private static StatementSyntax? GenerateConstantStatement(GeneratorInput input, Instruction instruction, string flagsVariableName)
    {
        var constants = GetConstantExpressions(input, instruction);
        if (constants.Count == 0)
        {
            return null;
        }

        string comment;
        if (constants.TryGetValue(0, out var resetFlags))
        {
            comment = $"// Reset {string.Join("", resetFlags)}.";
        }
        else
        {
            comment = "//";
        }

        byte bitMask;
        if (constants.TryGetValue(1, out var setFlags))
        {
            bitMask = BuildBitMask(setFlags);
            comment += $" Set {string.Join("", setFlags)}.";
        }
        else
        {
            bitMask = 0;
        }

        return CreateInitialize(flagsVariableName, Comment(comment), GenerateBinaryLiteralExpression(bitMask));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCopyFromStatements(GeneratorInput input, Instruction instruction, string flagsVariableName, bool initialized)
    {
        var copyFroms = GetCopyFromExpressions(input, instruction);

        foreach (var kvp in copyFroms.OrderBy(kvp => kvp.Key == input.FlagsRegister.FieldName ? 0 : 1).ThenBy(kvp => kvp.Key))
        {
            var comment = Comment($"// Copy {string.Join("", kvp.Value)} from {kvp.Key}.");
            var copyFromExpression = BinaryExpression(SyntaxKind.BitwiseAndExpression, IdentifierName(kvp.Key), GenerateBinaryLiteralExpression(BuildBitMask(kvp.Value)));

            if (initialized)
            {
                yield return CreateFlagsOrAssignment(flagsVariableName, comment, copyFromExpression);
            }
            else
            {
                yield return CreateInitialize(flagsVariableName, comment, copyFromExpression);
                initialized = true;
            }
        }
    }

    private static StatementSyntax CreateInitialize(string flagsVariableName, SyntaxTrivia comment, ExpressionSyntax expression) =>
        LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(Identifier(flagsVariableName))
                        .WithInitializer(EqualsValueClause(expression)))))
            .WithLeadingTrivia(Comment("// Flags."))
            .WithTrailingTrivia(comment);

    [Pure]
    private static StatementSyntax CreateFlagsOrAssignment(string flagsVariableName, string comment, ExpressionSyntax expression) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.OrAssignmentExpression, IdentifierName(flagsVariableName), expression))
        .WithTrailingTrivia(Comment(comment));

    [Pure]
    private static StatementSyntax CreateFlagsOrAssignment(string flagsVariableName, SyntaxTrivia comment, ExpressionSyntax expression) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.OrAssignmentExpression, IdentifierName(flagsVariableName), expression))
        .WithTrailingTrivia(comment);

    [Pure]
    private static IReadOnlyDictionary<string, List<Flag>> GetCopyFromExpressions(GeneratorInput input, Instruction instruction)
    {
        var copyFroms = new Dictionary<string, List<Flag>>();
        foreach (var flag in input.Flags.Values.OrderByDescending(f => f.Index))
        {
            if (instruction.Flags.TryGetValue(flag.Name, out var expression))
            {
                if (expression is Call call && call.Function == PreDefinedFunction.CopyFrom)
                {
                    var registerAccess = call.Arguments[0] as RegisterAccess ?? throw new InvalidOperationException("Can only copy_from registers.");
                    Add(copyFroms, registerAccess.Register.Name, flag);
                }
            }
            else
            {
                Add(copyFroms, input.FlagsRegister.Name, flag);
            }
        }

        return copyFroms;
    }
    [Pure]
    private static IReadOnlyDictionary<int, List<Flag>> GetConstantExpressions(GeneratorInput input, Instruction instruction)
    {
        var constants = new Dictionary<int, List<Flag>>();
        foreach (var flag in input.Flags.Values.OrderByDescending(f => f.Index))
        {
            if (instruction.Flags.TryGetValue(flag.Name, out var expression) && expression is Number number)
            {
                Add(constants, number.Value, flag);
            }
        }

        return constants;
    }

    [Pure]
    private static IEnumerable<(Flag Flag, Call Expression)> EnumerateCallExpressions(GeneratorInput input, Instruction instruction)
    {
        foreach (var kvp in instruction.Flags)
        {
            if (kvp.Value is Call call && call.Function != PreDefinedFunction.CopyFrom)
            {
                yield return (input.Flags[kvp.Key], call);
            }
        }
    }

    [Pure]
    private static byte BuildBitMask([InstantHandle] IEnumerable<Flag> flags) => (byte)flags.Aggregate(0, (current, flag) => current | (1 << flag.Index));

    [Pure]
    private static byte BuildBitMask(Flag flag) => (byte)(1 << flag.Index);

    private static void Add<TKey, TValue>(Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
    {
        if (!dictionary.TryGetValue(key, out var values))
        {
            values = [];
            dictionary.Add(key, values);
        }
        values.Add(value);
    }
}