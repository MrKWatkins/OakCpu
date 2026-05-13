using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class StatementGenerator
{
    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(FileGeneratorContext input, Step step)
    {
        var context = new StatementGeneratorContext(input, step);

        foreach (var statement in GenerateStepStatements(context, step))
        {
            yield return statement;
        }

        foreach (var statement in StatementBoundaryEmitter.GenerateBoundaryStatements(context))
        {
            yield return statement;
        }

        foreach (var statement in GenerateTrailingStatements(context, step))
        {
            yield return statement;
        }
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateStatements(FileGeneratorContext context, IEnumerable<Statement> statements, bool instructionEmulatorMode = false)
    {
        var statementContext = new StatementGeneratorContext(context, null);
        if (instructionEmulatorMode)
        {
            statementContext = statementContext.WithInstructionEmulatorMode();
        }

        return statements.SelectMany(statement => GenerateStatement(statementContext, statement));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateInstructionCompletionStatements(FileGeneratorContext context, IEnumerable<Statement> statements, string instructionUpdatesFlagsParameterName)
    {
        var statementContext = new StatementGeneratorContext(context, null).WithInstructionCompletionMode(instructionUpdatesFlagsParameterName);
        return statements.SelectMany(statement => GenerateStatement(statementContext, statement));
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateOverlapStatements(FileGeneratorContext input, Step step)
        => GenerateOverlapStatements(input, step, 0);

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateOverlapStatements(FileGeneratorContext input, Step step, int trailingStatementsToSkip)
    {
        var context = new StatementGeneratorContext(input, step).WithoutHandleInterrupts();
        var stepLayout = input.GeneratorContext.GetStepLayout(step);

        if (stepLayout.DoesNothing)
        {
            throw new InvalidOperationException("Trying to generate overlap statements for a step that does nothing.");
        }

        foreach (var statement in GenerateStepStatements(context, step, trailingStatementsToSkip))
        {
            yield return statement;
        }
    }

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateInstructionStatements(
        FileGeneratorContext input,
        Step step,
        string? nextInstructionVariableName,
        Step? instructionExitOverlapStep,
        int instructionTStatesBeforeStep,
        int trailingStatementsToSkip)
    {
        var context = new StatementGeneratorContext(input, step).WithInstructionStepMode(nextInstructionVariableName, instructionExitOverlapStep, instructionTStatesBeforeStep);
        var stepLayout = input.GeneratorContext.GetStepLayout(step);

        if (stepLayout.DoesNothing)
        {
            throw new InvalidOperationException("Trying to generate statements for a step that does nothing.");
        }

        foreach (var statement in GenerateStepStatements(context, step, trailingStatementsToSkip))
        {
            yield return statement;
        }

        foreach (var statement in StatementBoundaryEmitter.GenerateBoundaryStatements(context))
        {
            yield return statement;
        }

        foreach (var statement in GenerateTrailingStatements(context, step))
        {
            yield return statement;
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateTrailingStatements(StatementGeneratorContext context, Step step)
    {
        var stepLayout = context.GeneratorContext.GetStepLayout(step);

        return
        !context.Mode.EmitsTrailingStatements
            ? []
            : stepLayout.NextOpcode switch
            {
                NextOpcodeMode.Read => StatementTransitionEmitter.GenerateMoveToSequenceStart(context.GeneratorContext, context.GeneratorContext.OpcodeRead),
                NextOpcodeMode.Overlapped when stepLayout.Sequence is PrefixJump => StatementTransitionEmitter.GenerateExecuteSequenceOnStart(context.GeneratorContext, context.GeneratorContext.OpcodeRead, "Overlapped opcode read."),
                NextOpcodeMode.Overlapped => [],
                NextOpcodeMode.Custom => [],
                NextOpcodeMode.Loop => [],
                null => [],
                _ => throw new NotSupportedException($"The next opcode mode {stepLayout.NextOpcode} is not supported.")
            };
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStepStatements(StatementGeneratorContext context, Step step, int trailingStatementsToSkip = 0)
    {
        var stepLayout = context.GeneratorContext.GetStepLayout(step);

        if (stepLayout.RequiresPrefixReset)
        {
            yield return StatementTransitionEmitter.GenerateSetOpcodeStepTable(context.Configuration.OpcodeStepTables.NoPrefix);
        }

        if (stepLayout.ExecutesStoredOverlapOnStart && context.Mode.ExecutesStoredOverlapOnStart)
        {
            yield return StatementTransitionEmitter.GenerateExecuteOverlap().WithLeadingTrivia(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Comment("// Execute queued overlap."));
        }

        foreach (var stepStatement in trailingStatementsToSkip == 0 ? step.Statements : step.Statements.Take(step.Statements.Count - trailingStatementsToSkip))
        {
            foreach (var statement in GenerateStatement(context, stepStatement))
            {
                yield return statement;
            }
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> GenerateStatement(StatementGeneratorContext context, Statement statement) =>
        StatementStatementEmitter.Generate(context, statement);
}