using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

internal static class StatementAssignmentEmitter
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateTemporaryVariableDeclaration(StatementGeneratorContext context, TemporaryVariable temporaryVariable)
    {
        if (!context.InitializedTemporaryVariables.Add(temporaryVariable.Name))
        {
            throw new InvalidOperationException($"The temporary variable {temporaryVariable.Name} has already been initialized.");
        }

        yield return LocalDeclarationStatement(VariableDeclaration(temporaryVariable.Type.TypeSyntax())
            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(temporaryVariable.Name)))));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateAssignment(StatementGeneratorContext context, Assignment assignment)
    {
        if (assignment.Target == assignment.Value)
        {
            yield break;
        }

        if (ShouldSkipInstructionCurrentStepAssignment(context, assignment))
        {
            yield break;
        }

        if (TryGenerateCompoundAssignment(context, assignment, out var statement))
        {
            yield return statement;
            yield break;
        }

        var value = ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Value);
        if (assignment.Target.Type != assignment.Value.Type && assignment.Value is not Number)
        {
            if (assignment.Value is BinaryOperation)
            {
                value = ParenthesizedExpression(value);
            }

            value = CastExpression(assignment.Target.TypeSyntax, value);
        }

        ExpressionSyntax target;
        if (assignment.Target is TemporaryVariableAccess temporaryVariableAccess)
        {
            if (context.InitializedTemporaryVariables.Add(temporaryVariableAccess.Name))
            {
                yield return InitializeVariableStatement(temporaryVariableAccess.Name, value);
                yield break;
            }

            target = temporaryVariableAccess.Identifier;
        }
        else
        {
            target = ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Target);
        }

        yield return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, target, value));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateIf(StatementGeneratorContext context, IfStatement ifStatement)
    {
        var condition = ExpressionGenerator.GenerateExpressionSyntax(context.WithBooleanContext(), ifStatement.Condition);

        var ifContext = context.WithChildVariableScope();
        var ifStatements = ifStatement.IfStatements.SelectMany(statement => StatementStatementEmitter.Generate(ifContext, statement));

        var elseContext = context.WithChildVariableScope();
        var elseStatements = ifStatement.ElseStatements.SelectMany(statement => StatementStatementEmitter.Generate(elseContext, statement));

        if (condition is LiteralExpressionSyntax literal)
        {
            var constant = (bool)literal.Token.Value!;
            return constant ? ifStatements : elseStatements;
        }

        return ifStatement.ElseStatements.Any()
            ? [IfStatement(condition, Block(ifStatements), ElseClause(Block(elseStatements)))]
            : [IfStatement(condition, Block(ifStatements))];
    }

    [Pure]
    private static bool ShouldSkipInstructionCurrentStepAssignment(StatementGeneratorContext context, Assignment assignment)
    {
        if (!context.InstructionStepMode ||
            assignment.Target is not DataMemberAccess { DataMember: var dataMember } ||
            dataMember != PreDefinedDataMember.CurrentStep ||
            assignment.Value is not Number number)
        {
            return false;
        }

        return number.Value switch
        {
            0 => true,
            1 => true,
            _ => throw new InvalidOperationException("The instruction emulator only supports current_step assignments of 0 or 1.")
        };
    }

    [Pure]
    private static bool TryGenerateCompoundAssignment(StatementGeneratorContext context, Assignment assignment, [MaybeNullWhen(false)] out StatementSyntax statement)
    {
        if (assignment.Value is not BinaryOperation binary ||
            binary.Left != assignment.Target ||
            binary.Operator.CompoundAssignmentSyntaxKind == null)
        {
            statement = null;
            return false;
        }

        if (assignment.Target.Type != binary.Right.Type && binary.Right is not Number)
        {
            statement = null;
            return false;
        }

        var value = ExpressionGenerator.GenerateExpressionSyntax(context, binary.Right);
        var target = assignment.Target is TemporaryVariableAccess temporaryVariableAccess ? temporaryVariableAccess.Identifier : ExpressionGenerator.GenerateExpressionSyntax(context, assignment.Target);

        statement = ExpressionStatement(AssignmentExpression(binary.Operator.CompoundAssignmentSyntaxKind.Value, target, value));
        return true;
    }
}