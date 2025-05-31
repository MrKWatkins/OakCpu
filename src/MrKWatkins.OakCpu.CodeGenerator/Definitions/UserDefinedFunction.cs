using MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Expressions.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedFunction : Function
{
    private UserDefinedFunction(string name, Type type, IReadOnlyList<string> parameters, bool isBooleanLike, Expression expression)
        : base(name, type, parameters)
    {
        IsBooleanLike = isBooleanLike;
        Expression = expression;
    }

    public bool IsBooleanLike { get; }

    public Expression Expression { get; }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)} => {Expression}";

    [Pure]
    public static IReadOnlyList<UserDefinedFunction> Create(ParserContext context, IReadOnlyList<FunctionYaml> yamls) =>
        yamls.Select(y => Create(context, y)).OrderBy(f => f.Name).ToList();

    [Pure]
    public static UserDefinedFunction Create(ParserContext context, FunctionYaml yaml)
    {
        var parmeters = new HashSet<string>(yaml.Parameters.Select(a => a.Name));

        context = context.WithArguments(parmeters);
        var expression = ExpressionParser.ParseExpression(context, yaml.Expression);

        var (type, isBooleanLike) = yaml.Type switch
        {
            "int_bool" => (typeof(int), true),
            _ => throw new NotSupportedException($"The function type {yaml.Type} is not supported.")
        };

        return new UserDefinedFunction(yaml.Name, type, parmeters.ToList(), isBooleanLike, expression);
    }
}