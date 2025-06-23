using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StatementGeneratorContext
{
    public StatementGeneratorContext(GeneratorContext generatorContext, Step? step)
        : this(generatorContext, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, false)
    {
    }

    private StatementGeneratorContext(GeneratorContext generatorContext, Step? step, HashSet<string> initializedTemporaryVariables, ImmutableDictionary<string, Expression> argumentScope, bool inBooleanContext)
    {
        GeneratorContext = generatorContext;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        InBooleanContext = inBooleanContext;
    }

    public GeneratorContext GeneratorContext { get; }

    public Configuration Configuration => GeneratorContext.Configuration;

    public Step? Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public bool InBooleanContext { get; }

    [Pure]
    public StatementGeneratorContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)).Where(t => !ArgumentScope.ContainsKey(t.Key)));

        return new StatementGeneratorContext(GeneratorContext, Step, InitializedTemporaryVariables, newScope, InBooleanContext);
    }

    [Pure]
    public StatementGeneratorContext WithBooleanContext() => new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, true);
}