using System.Collections.Immutable;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StatementGeneratorContext
{
    public StatementGeneratorContext(GeneratorContext generatorContext, Step? step)
        : this(generatorContext, step, new HashSet<string>(), ImmutableDictionary<string, Expression>.Empty, false, null, false)
    {
    }

    private StatementGeneratorContext(GeneratorContext generatorContext, Step? step, HashSet<string> initializedTemporaryVariables, ImmutableDictionary<string, Expression> argumentScope, bool inBooleanContext, Expression? parent, bool skipHandleInterrupts)
    {
        GeneratorContext = generatorContext;
        Step = step;
        InitializedTemporaryVariables = initializedTemporaryVariables;
        ArgumentScope = argumentScope;
        InBooleanContext = inBooleanContext;
        Parent = parent;
        SkipHandleInterrupts = skipHandleInterrupts;
    }

    public GeneratorContext GeneratorContext { get; }

    public Configuration Configuration => GeneratorContext.Configuration;

    public Step? Step { get; }

    public HashSet<string> InitializedTemporaryVariables { get; }

    public ImmutableDictionary<string, Expression> ArgumentScope { get; }

    public bool InBooleanContext { get; }

    public Expression? Parent { get; }

    public bool SkipHandleInterrupts { get; }

    [Pure]
    public StatementGeneratorContext WithArguments(IEnumerable<string> parameters, IEnumerable<Expression> arguments)
    {
        var newScope = ArgumentScope.AddRange(parameters.Zip(arguments, (p, a) => new KeyValuePair<string, Expression>(p, a)).Where(t => !ArgumentScope.ContainsKey(t.Key)));

        return new StatementGeneratorContext(GeneratorContext, Step, InitializedTemporaryVariables, newScope, InBooleanContext, Parent, SkipHandleInterrupts);
    }

    [Pure]
    public StatementGeneratorContext WithBooleanContext() => new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, true, Parent, SkipHandleInterrupts);

    [Pure]
    public StatementGeneratorContext WithChildVariableScope() => new(GeneratorContext, Step, [.. InitializedTemporaryVariables], ArgumentScope, InBooleanContext, Parent, SkipHandleInterrupts);

    [Pure]
    public StatementGeneratorContext WithParentExpression(Expression parent) => new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, InBooleanContext, parent, SkipHandleInterrupts);

    [Pure]
    public StatementGeneratorContext WithoutHandleInterrupts() => new(GeneratorContext, Step, InitializedTemporaryVariables, ArgumentScope, InBooleanContext, Parent, true);
}