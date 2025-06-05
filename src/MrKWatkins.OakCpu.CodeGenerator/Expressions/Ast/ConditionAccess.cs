using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class ConditionAccess(Condition condition) : Access(condition.Name)
{
    public Condition Condition { get; } = condition;

    public override Type Type => typeof(bool);

    public override TypeSyntax TypeSyntax => throw new NotSupportedException($"{nameof(TypeSyntax)} is not supported for {nameof(ConditionAccess)}.");

    public override IdentifierNameSyntax Identifier => throw new NotSupportedException($"{nameof(Identifier)} is not supported for {nameof(ConditionAccess)}.");
}