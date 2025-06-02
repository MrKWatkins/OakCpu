using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class TemporaryVariableAccess(TemporaryVariable temporaryVariable) : Access(temporaryVariable.Name)
{
    public TemporaryVariable TemporaryVariable { get; } = temporaryVariable;

    public override Type Type => TemporaryVariable.Type;

    public override TypeSyntax TypeSyntax => TemporaryVariable.TypeSyntax;
}