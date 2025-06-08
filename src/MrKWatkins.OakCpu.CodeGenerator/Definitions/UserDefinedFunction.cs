using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedFunction : Function
{
    private UserDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters, Expression expression)
        : base(name, type, parameters)
    {
        Expression = expression;
    }

    public Expression Expression { get; }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)} => {Expression}";

    [Pure]
    public static IReadOnlyList<UserDefinedFunction> Create(ParserContext context, IReadOnlyList<FunctionYaml> yamls) =>
        yamls.Select(y => Create(context, y)).OrderBy(f => f.Name).ToList();

    [Pure]
    private static UserDefinedFunction Create(ParserContext context, FunctionYaml yaml)
    {
        var parameters = new HashSet<string>(yaml.Parameters);

        context = context.WithArguments(parameters);
        var expression = Parser.ParseExpression(context, yaml.Expression);

        return new UserDefinedFunction(yaml.Name, YamlSerializer.Deserialize<DataType>(Encoding.UTF8.GetBytes(yaml.Type)), parameters.ToList(), expression);
    }
}