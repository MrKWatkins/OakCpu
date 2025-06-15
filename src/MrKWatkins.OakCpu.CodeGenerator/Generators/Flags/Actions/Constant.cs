using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

internal sealed class Constant : FlagAction
{
    private Constant(Flag flag, bool set)
        : base(flag)
    {
        BitMask = set ? flag.BitMask : (byte)0;
    }

    internal Constant(IReadOnlyList<Flag> flags, byte bitMask)
        : base(flags)
    {
        BitMask = bitMask;
    }

    internal byte BitMask { get; }

    internal override int Order => 0;

    internal override ExpressionSyntax GenerateExpression(StepContext context) => GenerateBinaryLiteralExpression(BitMask);

    internal override string GenerateComment(StepContext context)
    {
        var sets = new List<Flag>();
        var resets = new List<Flag>();
        foreach (var flag in Flags)
        {
            if ((BitMask & flag.BitMask) == 0)
            {
                resets.Add(flag);
            }
            else
            {
                sets.Add(flag);
            }
        }

        var comment = "//";
        if (sets.Any())
        {
            comment += $" Set {FlagsNames(sets)}.";
        }
        if (resets.Any())
        {
            comment += $" Reset {FlagsNames(resets)}.";
        }

        return comment;
    }

    [Pure]
    internal static FlagAction? CreateOrNull(Flag flag, Expression expression)
    {
        if (expression is Number number)
        {
            var set = number.Value switch
            {
                1 => true,
                0 => false,
                _ => throw new ArgumentException($"Invalid value for flag {flag.Name} setter.", nameof(expression))
            };
            return new Constant(flag, set);
        }

        return null;
    }
}