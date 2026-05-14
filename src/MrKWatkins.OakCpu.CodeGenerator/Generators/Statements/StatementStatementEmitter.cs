using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Statements;

internal static class StatementStatementEmitter
{
    [Pure]
    public static IEnumerable<StatementSyntax> Generate(StatementGeneratorContext context, Statement statement) =>
        statement switch
        {
            Assignment assignment => StatementAssignmentEmitter.GenerateAssignment(context, assignment),
            IfStatement ifStatement => StatementAssignmentEmitter.GenerateIf(context, ifStatement),
            CallStatement callStatement => StatementCallEmitter.GenerateCall(context, callStatement),
            TemporaryVariableDeclarationStatement temporaryVariableDeclaration => StatementAssignmentEmitter.GenerateTemporaryVariableDeclaration(context, temporaryVariableDeclaration.Variable),
            _ => throw new NotSupportedException($"The statement type {statement.GetType().Name} is not supported.")
        };
}