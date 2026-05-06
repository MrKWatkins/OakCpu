using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedFunction : Function
{
    private Expression? expression;

    private UserDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public Expression Expression => expression ?? throw new InvalidOperationException($"{nameof(Expression)} not set.");

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)} => {Expression}";

    public static IReadOnlyDictionary<string, UserDefinedFunction> CreateDeclarations(IReadOnlyList<FunctionYaml> yamls) =>
        yamls.Select(
                yaml => new UserDefinedFunction(
                    yaml.Name,
                    YamlSerializer.Deserialize<DataType>(Encoding.UTF8.GetBytes(yaml.Type)),
                    yaml.Parameters))
            .ToDictionary(function => function.Name, function => function);

    public static void ParseExpressions(Configuration configuration, IReadOnlyDictionary<string, UserDefinedFunction> functions, IReadOnlyList<FunctionYaml> yamls)
    {
        var context = new ParserContext(configuration);
        foreach (var yaml in yamls)
        {
            functions[yaml.Name].SetExpression(Parser.ParseExpression(context.WithArguments(yaml.Parameters), yaml.Expression));
        }
    }

    private void SetExpression(Expression value) => expression = value;
}