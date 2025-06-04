using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class FlagAccess(Flag flag) : Access(flag.Name)
{
    public Flag Flag { get; } = flag;

    public override Type Type => typeof(int);

    public override TypeSyntax TypeSyntax => throw new NotSupportedException($"{nameof(TypeSyntax)} is not supported for {nameof(FlagAccess)}.");

    public override IdentifierNameSyntax Identifier => throw new NotSupportedException($"{nameof(Identifier)} is not supported for {nameof(FlagAccess)}.");
}