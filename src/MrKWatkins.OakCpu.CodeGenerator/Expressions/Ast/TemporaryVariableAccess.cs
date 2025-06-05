using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class TemporaryVariableAccess(string name) : Access(name)
{
    public override Type Type => typeof(int);

    public override TypeSyntax TypeSyntax => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
}