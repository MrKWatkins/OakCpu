using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StepContext
{
    public StepContext(GeneratorContext context, Step step)
        : this(context, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, new List<string>(), false)
    {
    }

    private StepContext(GeneratorContext context, Step step, HashSet<string> initializedTemporaryVariables, ImmutableDictionary<string, Expression> argumentScope, List<string> commentsAheadOfNextStatement, bool inBooleanContext)
    {
        Context = context;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        CommentsAheadOfNextStatement = commentsAheadOfNextStatement;
        InBooleanContext = inBooleanContext;
    }

    public GeneratorContext Context { get; }

    public Configuration Configuration => Context.Configuration;

    public Step Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public List<string> CommentsAheadOfNextStatement { get; }

    public bool InBooleanContext { get; }

    [Pure]
    public StepContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)).Where(t => !ArgumentScope.ContainsKey(t.Key)));

        return new StepContext(Context, Step, InitializedTemporaryVariables, newScope, CommentsAheadOfNextStatement, InBooleanContext);
    }

    [Pure]
    public StepContext WithBooleanContext() => new(Context, Step, InitializedTemporaryVariables, ArgumentScope, CommentsAheadOfNextStatement, true);
}