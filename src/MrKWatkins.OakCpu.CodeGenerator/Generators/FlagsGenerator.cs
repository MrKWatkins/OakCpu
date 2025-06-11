using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class FlagsGenerator : Generator
{
    private const string FlagsVariableName = "flags";

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateFlagsStatements(StepContext context)
    {
        // TODO: Copy fields used in statements to a local variable first for a performance optimisation.
        var instruction = context.Step.Instruction ?? throw new InvalidOperationException("Cannot use flags() outside of an instruction.");

        var handled = new HashSet<Flag>();
        var constants = GenerateConstantStatement(context, instruction, handled);
        if (constants != null)
        {
            yield return constants;
        }

        foreach (var statement in GenerateCopyFromStatements(context, instruction, handled))
        {
            yield return statement;
        }

        foreach (var statement in GenerateExpressionStatements(context, instruction, handled))
        {
            yield return statement;
        }

        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(context.Configuration.FlagsRegister.FieldName),
                CastExpression(
                    context.Configuration.FlagsRegister.TypeSyntax, IdentifierName(FlagsVariableName))));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateExpressionStatements(StepContext context, Instruction instruction, HashSet<Flag> handled)
    {
        foreach (var flag in context.Configuration.Flags.Values.Where(f => !handled.Contains(f)).OrderByDescending(f => f.Index))
        {
            var expression = instruction.Flags[flag.Name];
            if (expression.Type != DataType.Bool && expression.Type != DataType.I32Bool)
            {
                throw new InvalidOperationException($"Flags expression {expression} is not of type {nameof(DataType.Bool)} or {nameof(DataType.I32Bool)}.");
            }

            var expressionSyntax = ExpressionGenerator.GenerateExpressionSyntax(context, expression);

            if (expression.Type == DataType.Bool)
            {
                var bitMask = BuildBitMask(flag);

                yield return IfStatement(expressionSyntax, Block(CreateFlagsOrAssignment($"// Set {flag} when {expression} is true.", GenerateBinaryLiteralExpression(bitMask))));
            }
            else
            {
                var shiftExpressionSyntax = flag.Index != 0
                    ? BinaryExpression(SyntaxKind.LeftShiftExpression, ParenthesizedExpression(expressionSyntax), GenerateNumericLiteralExpression(flag.Index))
                    : expressionSyntax;

                yield return CreateFlagsOrAssignment($"// Set {flag} when {shiftExpressionSyntax} is 0x01.", shiftExpressionSyntax);
            }
        }
    }


    [Pure]
    private static StatementSyntax GenerateIsNegativeStatement(StepContext context, Flag flag, Call call)
    {
        var oneAtBit7IfNegative = BinaryExpression(SyntaxKind.BitwiseAndExpression, ExpressionGenerator.GenerateExpressionSyntax(context, call.Arguments[0]), GenerateBinaryLiteralExpression(0b10000000));

        var shift = 7 - flag.Index;
        var expression = shift > 0
            ? BinaryExpression(SyntaxKind.RightShiftExpression, ParenthesizedExpression(oneAtBit7IfNegative), GenerateNumericLiteralExpression(shift))
            : oneAtBit7IfNegative;

        return CreateFlagsOrAssignment($"// Set {flag} when {call.Arguments[0]} is negative.", expression);
    }

    [Pure]
    private static StatementSyntax GenerateIsZeroStatement(StepContext context, Flag flag, Call call)
    {
        var condition = BinaryExpression(SyntaxKind.EqualsExpression, ExpressionGenerator.GenerateExpressionSyntax(context, call.Arguments[0]), GenerateNumericLiteralExpression(0));

        var bitMask = BuildBitMask(flag);

        return IfStatement(condition, Block(CreateFlagsOrAssignment($"// Set {flag} when {call.Arguments[0]} is zero.", GenerateBinaryLiteralExpression(bitMask))));
    }

    [Pure]
    private static StatementSyntax? GenerateConstantStatement(StepContext context, Instruction instruction, HashSet<Flag> handled)
    {
        var constants = GetConstantExpressions(context, instruction);
        if (constants.Count == 0)
        {
            return null;
        }

        var comment = constants.TryGetValue(0, out var resetFlags) ? $"// Reset {FlagsNames(resetFlags)}." : "//";

        byte bitMask;
        if (constants.TryGetValue(1, out var setFlags))
        {
            bitMask = BuildBitMask(setFlags);
            comment += $" Set {FlagsNames(setFlags)}.";
        }
        else
        {
            bitMask = 0;
        }

        foreach (var flag in constants.Values.SelectMany(v => v))
        {
            handled.Add(flag);
        }

        return CreateInitialize(Comment(comment), GenerateBinaryLiteralExpression(bitMask));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCopyFromStatements(StepContext context, Instruction instruction, HashSet<Flag> handled)
    {
        var copyFroms = GetCopyFromExpressions(context, instruction);

        foreach (var kvp in copyFroms.OrderBy(kvp => kvp.Key == context.Configuration.FlagsRegister.FieldName ? 0 : 1).ThenBy(kvp => kvp.Key))
        {
            var comment = Comment($"// Copy {FlagsNames(kvp.Value)} from {kvp.Key}.");
            var copyFromExpression = BinaryExpression(SyntaxKind.BitwiseAndExpression, IdentifierName(kvp.Key), GenerateBinaryLiteralExpression(BuildBitMask(kvp.Value)));

            if (handled.Any())
            {
                yield return CreateFlagsOrAssignment(comment, copyFromExpression);
            }
            else
            {
                yield return CreateInitialize(comment, copyFromExpression);
            }

            foreach (var flag in kvp.Value)
            {
                handled.Add(flag);
            }
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateAssignmentFromEqualityStatements(StepContext context, Instruction instruction)
    {
        foreach (var flag in context.Configuration.Flags.Values.OrderByDescending(f => f.Index))
        {
            if (instruction.Flags.TryGetValue(flag.Name, out var expression) && expression is BinaryOperation binaryOperation && binaryOperation.Operator == Operator.Equality)
            {
                yield return GenerateAssignmentFromEqualityStatement(context, flag, binaryOperation);
            }
        }
    }

    [Pure]
    private static StatementSyntax GenerateAssignmentFromEqualityStatement(StepContext context, Flag flag, BinaryOperation equality)
    {
        var bitMask = BuildBitMask(flag);

        var ternary = ConditionalExpression(
            ExpressionGenerator.GenerateExpressionSyntax(context, equality),
            GenerateBinaryLiteralExpression(bitMask),
            GenerateNumericLiteralExpression(0));

        return CreateFlagsOrAssignment($"// Set {flag} when {equality}.", ternary);
    }

    [Pure]
    private static StatementSyntax GenerateExpressionStatement(StepContext context, Flag flag, Expression expression)
    {
        var bitMask = BuildBitMask(flag);

        var ternary = ConditionalExpression(
            ExpressionGenerator.GenerateExpressionSyntax(context, expression),
            GenerateBinaryLiteralExpression(bitMask),
            GenerateNumericLiteralExpression(0));

        return CreateFlagsOrAssignment($"// Set {flag} when {expression}.", ternary);
    }

    private static StatementSyntax CreateInitialize(SyntaxTrivia comment, ExpressionSyntax expression) =>
        InitializeVariableStatement(FlagsVariableName, expression)
            .WithLeadingTrivia(Comment("// Flags."))
            .WithTrailingTrivia(comment);

    [Pure]
    private static StatementSyntax CreateFlagsOrAssignment(string comment, ExpressionSyntax expression) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.OrAssignmentExpression, IdentifierName(FlagsVariableName), expression))
        .WithTrailingTrivia(Comment(comment));

    [Pure]
    private static StatementSyntax CreateFlagsOrAssignment(SyntaxTrivia comment, ExpressionSyntax expression) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.OrAssignmentExpression, IdentifierName(FlagsVariableName), expression))
        .WithTrailingTrivia(comment);

    [Pure]
    private static IReadOnlyDictionary<string, List<Flag>> GetCopyFromExpressions(StepContext context, Instruction instruction)
    {
        var copyFroms = new Dictionary<string, List<Flag>>();
        foreach (var flag in context.Configuration.Flags.Values.OrderByDescending(f => f.Index))
        {
            if (instruction.Flags.TryGetValue(flag.Name, out var expression))
            {
                if (expression is Call call && call.Function == PreDefinedFunction.CopyFrom)
                {
                    var access = call.Arguments[0] as Access ?? throw new InvalidOperationException("Can only copy_from registers.");

                    Add(copyFroms, access.Name, flag);
                }
            }
            else
            {
                Add(copyFroms, context.Configuration.FlagsRegister.Name, flag);
            }
        }

        return copyFroms;
    }

    [Pure]
    private static IReadOnlyDictionary<int, List<Flag>> GetConstantExpressions(StepContext context, Instruction instruction)
    {
        var constants = new Dictionary<int, List<Flag>>();
        foreach (var flag in context.Configuration.Flags.Values.OrderByDescending(f => f.Index))
        {
            if (instruction.Flags.TryGetValue(flag.Name, out var expression) && expression is Number number)
            {
                Add(constants, number.Value, flag);
            }
        }

        return constants;
    }

    [Pure]
    private static IEnumerable<(Flag Flag, Call Expression)> EnumerateCallExpressions(StepContext context, Instruction instruction)
    {
        foreach (var kvp in instruction.Flags)
        {
            if (kvp.Value is Call call && call.Function != PreDefinedFunction.CopyFrom)
            {
                yield return (context.Configuration.Flags[kvp.Key], call);
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

    [Pure]
    private static string FlagsNames([InstantHandle] IEnumerable<Flag> flags)
    {
        var flagsList = flags.ToList();

        return flagsList.Count switch
        {
            1 => flagsList[0].Name,
            2 => $"{flagsList[0].Name} and {flagsList[1].Name}",
            _ => $"{string.Join(", ", flagsList.Take(flagsList.Count - 1))} and {flagsList.Last().Name}"
        };
    }
}