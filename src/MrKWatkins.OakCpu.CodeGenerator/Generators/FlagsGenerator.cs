using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
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

        bool initialized;
        var constants = GenerateConstantStatement(context, instruction, FlagsVariableName);
        if (constants != null)
        {
            yield return constants;
            initialized = true;
        }
        else
        {
            initialized = false;
        }

        foreach (var statement in GenerateCopyFromStatements(context, instruction, FlagsVariableName, initialized))
        {
            yield return statement;
        }

        foreach (var statement in GenerateCallExpressionStatements(context, instruction, FlagsVariableName))
        {
            yield return statement;
        }

        foreach (var statement in GenerateAssignmentFromEqualityStatements(context, instruction, FlagsVariableName))
        {
            yield return statement;
        }

        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(context.Input.FlagsRegister.FieldName),
                CastExpression(
                    context.Input.FlagsRegister.TypeSyntax, IdentifierName(FlagsVariableName))));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCallExpressionStatements(StepContext context, Instruction instruction, string flagsVariableName)
    {
        foreach (var (flag, call) in EnumerateCallExpressions(context, instruction))
        {
            if (call.Function == PreDefinedFunction.IsNegative)
            {
                yield return GenerateIsNegativeStatement(context, flagsVariableName, flag, call);
            }
            if (call.Function == PreDefinedFunction.IsZero)
            {
                yield return GenerateIsZeroStatement(context, flagsVariableName, flag, call);
            }
            else if (call.Function is UserDefinedFunction userDefinedFunction)
            {
                yield return GenerateUserDefinedFunctionStatement(context, flagsVariableName, flag, call, userDefinedFunction);
            }
        }
    }

    [Pure]
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static StatementSyntax GenerateUserDefinedFunctionStatement(StepContext context, string flagsVariableName, Flag flag, Call call, UserDefinedFunction userDefinedFunction)
    {
        if (userDefinedFunction.Type == DataType.Bool)
        {
            throw new NotImplementedException("Bool user defined functions have not been implemented.");
        }
        if (userDefinedFunction.Type != DataType.I32Bool)
        {
            throw new InvalidOperationException("Flags can only be set from Boolean-like functions.");
        }

        var value = BinaryExpression(SyntaxKind.LeftShiftExpression, ExpressionGenerator.GenerateExpressionSyntax(context, call), GenerateNumericLiteralExpression(flag.Index));

        return CreateFlagsOrAssignment(flagsVariableName, $"// Set {flag} from {call}.", value);
    }

    [Pure]
    private static StatementSyntax GenerateIsNegativeStatement(StepContext context, string flagsVariableName, Flag flag, Call call)
    {
        var oneAtBit7IfNegative = BinaryExpression(SyntaxKind.BitwiseAndExpression, ExpressionGenerator.GenerateExpressionSyntax(context, call.Arguments[0]), GenerateBinaryLiteralExpression(0b10000000));

        var shift = 7 - flag.Index;
        var expression = shift > 0
            ? BinaryExpression(SyntaxKind.RightShiftExpression, ParenthesizedExpression(oneAtBit7IfNegative), GenerateNumericLiteralExpression(shift))
            : oneAtBit7IfNegative;

        return CreateFlagsOrAssignment(flagsVariableName, $"// Set {flag} when {call.Arguments[0]} is negative.", expression);
    }

    [Pure]
    private static StatementSyntax GenerateIsZeroStatement(StepContext context, string flagsVariableName, Flag flag, Call call)
    {
        var condition = BinaryExpression(SyntaxKind.EqualsExpression, ExpressionGenerator.GenerateExpressionSyntax(context, call.Arguments[0]), GenerateNumericLiteralExpression(0));

        var bitMask = BuildBitMask(flag);

        return IfStatement(condition, Block(CreateFlagsOrAssignment(flagsVariableName, $"// Set {flag} when {call.Arguments[0]} is zero.", GenerateBinaryLiteralExpression(bitMask))));
    }

    [Pure]
    private static StatementSyntax? GenerateConstantStatement(StepContext context, Instruction instruction, string flagsVariableName)
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

        return CreateInitialize(flagsVariableName, Comment(comment), GenerateBinaryLiteralExpression(bitMask));
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateCopyFromStatements(StepContext context, Instruction instruction, string flagsVariableName, bool initialized)
    {
        var copyFroms = GetCopyFromExpressions(context, instruction);

        foreach (var kvp in copyFroms.OrderBy(kvp => kvp.Key == context.Input.FlagsRegister.FieldName ? 0 : 1).ThenBy(kvp => kvp.Key))
        {
            var comment = Comment($"// Copy {FlagsNames(kvp.Value)} from {kvp.Key}.");
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

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateAssignmentFromEqualityStatements(StepContext context, Instruction instruction, string flagsVariableName)
    {
        foreach (var flag in context.Input.Flags.Values.OrderByDescending(f => f.Index))
        {
            if (instruction.Flags.TryGetValue(flag.Name, out var expression) && expression is BinaryOperation binaryOperation && binaryOperation.Operator == Operator.Equality)
            {
                yield return GenerateAssignmentFromEqualityStatement(context, flagsVariableName, flag, binaryOperation);
            }
        }
    }

    [Pure]
    private static StatementSyntax GenerateAssignmentFromEqualityStatement(StepContext context, string flagsVariableName, Flag flag, BinaryOperation equality)
    {
        var bitMask = BuildBitMask(flag);

        var ternary = ConditionalExpression(
            ExpressionGenerator.GenerateExpressionSyntax(context, equality),
            GenerateBinaryLiteralExpression(bitMask),
            GenerateNumericLiteralExpression(0));

        return CreateFlagsOrAssignment(flagsVariableName, $"// Set {flag} when {equality}.", ternary);
    }

    private static StatementSyntax CreateInitialize(string flagsVariableName, SyntaxTrivia comment, ExpressionSyntax expression) =>
        InitializeVariableStatement(flagsVariableName, expression)
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
    private static IReadOnlyDictionary<string, List<Flag>> GetCopyFromExpressions(StepContext context, Instruction instruction)
    {
        var copyFroms = new Dictionary<string, List<Flag>>();
        foreach (var flag in context.Input.Flags.Values.OrderByDescending(f => f.Index))
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
                Add(copyFroms, context.Input.FlagsRegister.Name, flag);
            }
        }

        return copyFroms;
    }

    [Pure]
    private static IReadOnlyDictionary<int, List<Flag>> GetConstantExpressions(StepContext context, Instruction instruction)
    {
        var constants = new Dictionary<int, List<Flag>>();
        foreach (var flag in context.Input.Flags.Values.OrderByDescending(f => f.Index))
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
                yield return (context.Input.Flags[kvp.Key], call);
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