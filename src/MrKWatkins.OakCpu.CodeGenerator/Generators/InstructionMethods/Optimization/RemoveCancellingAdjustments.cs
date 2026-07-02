using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.InstructionMethods.Optimization;

// Removes adjacent pairs of compound assignments to the same emulator field that cancel each other out, e.g.
// emulator.PC += 1; emulator.PC -= 1;. These arise when a step performs the overlapped read of the next opcode,
// e.g. for EI, and the instruction emulator rolls the program counter back again because it performs the read
// itself at the start of the next instruction. Nothing can observe the field between two adjacent statements, so
// removing the pair is safe.
internal sealed class RemoveCancellingAdjustments : InstructionMethodOptimization
{
    protected override IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements) => Optimize(statements);

    [Pure]
    private static IReadOnlyList<StatementSyntax> Optimize(IReadOnlyList<StatementSyntax> statements)
    {
        var result = new List<StatementSyntax>(statements.Count);
        for (var index = 0; index < statements.Count; index++)
        {
            if (index + 1 < statements.Count && Cancel(statements[index], statements[index + 1]))
            {
                index++;
                continue;
            }

            result.Add(OptimizeStatement(statements[index]));
        }

        return result;
    }

    [Pure]
    private static StatementSyntax OptimizeStatement(StatementSyntax statement) =>
        statement switch
        {
            BlockSyntax block => block.WithStatements(List(Optimize(block.Statements))),
            IfStatementSyntax ifStatement => ifStatement
                .WithStatement(OptimizeStatement(ifStatement.Statement))
                .WithElse(ifStatement.Else?.WithStatement(OptimizeStatement(ifStatement.Else.Statement))),
            _ => statement
        };

    [Pure]
    private static bool Cancel(StatementSyntax first, StatementSyntax second) =>
        TryGetAdjustment(first, out var firstField, out var firstAdjustment) &&
        TryGetAdjustment(second, out var secondField, out var secondAdjustment) &&
        firstField == secondField &&
        firstAdjustment == -secondAdjustment;

    [Pure]
    private static bool TryGetAdjustment(StatementSyntax statement, [NotNullWhen(true)] out string? field, out long adjustment)
    {
        field = null;
        adjustment = 0;

        if (statement is not ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment })
        {
            return false;
        }

        var sign = assignment.Kind() switch
        {
            SyntaxKind.AddAssignmentExpression => 1,
            SyntaxKind.SubtractAssignmentExpression => -1,
            _ => 0
        };
        if (sign == 0)
        {
            return false;
        }

        if (assignment.Left is not MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: Parameter.Name.Emulator },
                Name.Identifier.ValueText: var fieldName
            })
        {
            return false;
        }

        if (assignment.Right is not LiteralExpressionSyntax literal || GetValue(literal) is not { } value)
        {
            return false;
        }

        field = fieldName;
        adjustment = sign * value;
        return true;
    }

    [Pure]
    private static long? GetValue(LiteralExpressionSyntax literal) =>
        literal.Token.Value switch
        {
            byte value => value,
            sbyte value => value,
            short value => value,
            ushort value => value,
            int value => value,
            uint value => value,
            long value => value,
            _ => null
        };
}