using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;

public abstract class FlagsGenerator : Generator
{
    private const string FlagsVariableName = "flags";

    [Pure]
    public static IEnumerable<StatementSyntax> GenerateFlagsStatements(StatementGeneratorContext context)
    {
        if (context.Step?.Sequence is not Instruction instruction)
        {
            throw new InvalidOperationException("Cannot use flags() outside of an instruction.");
        }

        IReadOnlyList<FlagAction> actions = FlagAction.Create(context, instruction).ToList();

        var commentsBeforeInitialize = new List<string> { NewlineCommentText, "// Update flags." };
        actions = FlagOptimization.PerformAllOptimizations(context, actions, commentsBeforeInitialize);

        var initialized = false;
        foreach (var action in actions.OrderBy(a => a.Order))
        {
            var expression = action.GenerateExpression(context);
            var comment = action.GenerateComment();
            if (!initialized)
            {
                yield return CreateInitialize(expression, comment, commentsBeforeInitialize);
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
                EmulatorMemberIdentifier(context.Configuration.FlagsRegister.FieldName),
                CastExpression(
                    context.Configuration.FlagsRegister.TypeSyntax, IdentifierName(FlagsVariableName))));
    }

    private static StatementSyntax CreateInitialize(ExpressionSyntax expression, string comment, List<string> commentsBeforeInitialize) =>
        InitializeVariableStatement(FlagsVariableName, expression, IntType).WithLeadingTrivia(commentsBeforeInitialize.Select(Comment)).WithTrailingTrivia(Comment(comment));

    [Pure]
    private static StatementSyntax CreateFlagsOrAssignment(ExpressionSyntax expression, string comment) =>
        ExpressionStatement(AssignmentExpression(SyntaxKind.OrAssignmentExpression, IdentifierName(FlagsVariableName), expression))
            .WithTrailingTrivia(Comment(comment));
}