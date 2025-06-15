using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;

public abstract class FlagsGenerator : Generator
{
    private const string FlagsVariableName = "flags";

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateFlagsStatements(StepContext context)
    {
        var instruction = context.Step.Instruction ?? throw new InvalidOperationException("Cannot use flags() outside of an instruction.");

        IReadOnlyList<FlagAction> actions = FlagAction.Create(context, instruction).ToList();
        actions = FlagOptimization.PerformAllOptimizations(context, actions);

        var initialized = false;
        foreach (var action in actions.OrderBy(a => a.Order))
        {
            var expression = action.GenerateExpression(context);
            var comment = action.GenerateComment(context);
            if (!initialized)
            {
                yield return CreateInitialize(expression, comment);
                initialized = true;
            }
            else
            {
                yield return CreateFlagsOrAssignment(expression, comment);
            }
        }

        yield return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(context.Configuration.FlagsRegister.FieldName),
                CastExpression(
                    context.Configuration.FlagsRegister.TypeSyntax, IdentifierName(FlagsVariableName))));
    }

    private static StatementSyntax CreateInitialize(ExpressionSyntax expression, string comment) =>
        InitializeVariableStatement(FlagsVariableName, expression)
            .WithLeadingTrivia(Comment("// Flags."))
            .WithTrailingTrivia(Comment(comment));

    [Pure]
    private static StatementSyntax CreateFlagsOrAssignment(ExpressionSyntax expression, string comment) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.OrAssignmentExpression, IdentifierName(FlagsVariableName), expression))
            .WithTrailingTrivia(Comment(comment));
}