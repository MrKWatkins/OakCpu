using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

internal sealed class CopyFrom : FlagAction
{
    internal CopyFrom(Flag flag, Expression argument)
        : base(flag)
    {
        Argument = argument;
        BitMask = flag.BitMask;
    }

    internal CopyFrom(IReadOnlyList<Flag> flags, byte bitMask, Expression argument)
        : base(flags)
    {
        BitMask = bitMask;
        Argument = argument;
    }

    internal Expression Argument { get; }

    internal byte BitMask { get; }

    internal override int Order => 1;

    internal override ExpressionSyntax GenerateExpression(StatementGeneratorContext context) =>
        BinaryExpression(
            SyntaxKind.BitwiseAndExpression,
            ExpressionGenerator.GenerateExpressionSyntax(context, Argument),
            GenerateBinaryLiteralExpression(BitMask));

    internal override string GenerateComment() => $"// Copy {FlagsNames(Flags)} from {Argument}.";

    [Pure]
    internal static FlagAction? CreateOrNull(Flag flag, Expression expression) =>
        expression is Call call && call.Function == PreDefinedFunction.CopyFrom
            ? new CopyFrom(flag, call.Arguments.FirstOrDefault() ?? throw new InvalidOperationException("copy_from() must have at least one argument."))
            : null;
}