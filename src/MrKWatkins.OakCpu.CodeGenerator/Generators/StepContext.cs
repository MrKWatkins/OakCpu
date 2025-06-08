using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StepContext
{
    public StepContext(GeneratorInput input, Step step)
        : this(input, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, new List<string>(), false)
    {
    }

    private StepContext(GeneratorInput input, Step step, HashSet<string> initializedTemporaryVariables, ImmutableDictionary<string, Expression> argumentScope, List<string> commentsAheadOfNextStatement, bool inBooleanContext)
    {
        Input = input;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        CommentsAheadOfNextStatement = commentsAheadOfNextStatement;
        InBooleanContext = inBooleanContext;
    }

    public GeneratorInput Input { get; }

    public Step Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public List<string> CommentsAheadOfNextStatement { get; }

    public bool InBooleanContext { get; }

    [Pure]
    public StepContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)));

        return new StepContext(Input, Step, InitializedTemporaryVariables, newScope, CommentsAheadOfNextStatement, InBooleanContext);
    }

    [Pure]
    public StepContext WithBooleanContext() => new(Input, Step, InitializedTemporaryVariables, ArgumentScope, CommentsAheadOfNextStatement, true);
}