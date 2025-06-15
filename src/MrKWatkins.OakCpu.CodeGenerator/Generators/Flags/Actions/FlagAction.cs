using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;

internal abstract class FlagAction : Generator
{
    private protected FlagAction(Flag flag)
    {
        Flags = [flag];
    }

    private protected FlagAction(IReadOnlyList<Flag> flags)
    {
        Flags = flags;
    }

    internal IReadOnlyList<Flag> Flags { get; }

    internal abstract int Order { get; }

    [Pure]
    internal abstract ExpressionSyntax GenerateExpression(StepContext context);

    [Pure]
    internal abstract string GenerateComment(StepContext context);

    [Pure]
    public static IEnumerable<FlagAction> Create(StepContext context, Instruction instruction)
    {
        foreach (var kvp in instruction.Flags)
        {
            var flag = context.Configuration.Flags[kvp.Key];
            var action = Constant.CreateOrNull(flag, kvp.Value) ??
                         CopyFrom.CreateOrNull(flag, kvp.Value) ??
                         I32BoolExpression.CreateOrNull(flag, kvp.Value) ??
                         BoolExpression.CreateOrNull(flag, kvp.Value);

            if (action == null)
            {
                throw new InvalidOperationException($"Could not create action for flag {flag.Name} with value {kvp.Value}.");
            }
            yield return action;
        }

        // For every flag not mentioned in the instruction, copy the value from F to preserve it.
        foreach (var flag in context.Configuration.Flags.Values.Where(f => !instruction.Flags.ContainsKey(f.Name)))
        {
            yield return new CopyFrom(flag, new RegisterAccess(context.Configuration.FlagsRegister));
        }
    }

    [Pure]
    protected static string FlagsNames([InstantHandle] IReadOnlyList<Flag> flags)
    {
        return flags.Count switch
        {
            1 => flags[0].Name,
            2 => $"{flags[0].Name} and {flags[1].Name}",
            _ => $"{string.Join(", ", flags.Take(flags.Count - 1))} and {flags.Last().Name}"
        };
    }
}