using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StepContext
{
    public StepContext(GeneratorInput input, Step step)
        : this(input, step, new Dictionary<TemporaryVariable, string>(), ImmutableDictionary<string, Expression>.Empty, new List<string>())
    {
    }

    private StepContext(GeneratorInput input, Step step, Dictionary<TemporaryVariable, string> temporaryVariableNames, ImmutableDictionary<string, Expression> argumentScope, List<string> commentsAheadOfNextStatement)
    {
        Input = input;
        Step = step;
        TemporaryVariableNames = temporaryVariableNames;
        ArgumentScope = argumentScope;
        CommentsAheadOfNextStatement = commentsAheadOfNextStatement;
    }

    public GeneratorInput Input { get; }

    public Step Step { get; }

    public Dictionary<TemporaryVariable, string> TemporaryVariableNames { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public List<string> CommentsAheadOfNextStatement { get; }

    [Pure]
    public StepContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)));

        return new StepContext(Input, Step, TemporaryVariableNames, newScope, CommentsAheadOfNextStatement);
    }
}