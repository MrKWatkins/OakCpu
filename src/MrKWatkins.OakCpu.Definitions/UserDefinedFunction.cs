using System.Text;
using MrKWatkins.OakCpu.Definitions.Language.Ast;
using MrKWatkins.OakCpu.Definitions.Language.Parsing;
using MrKWatkins.OakCpu.Definitions.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.Definitions;

public sealed class UserDefinedFunction : Function
{
    private Expression? expression;

    private UserDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters, string? documentation, string? notDocumentation, string? valueDocumentation)
        : base(name, type, parameters)
    {
        Documentation = documentation;
        NotDocumentation = notDocumentation;
        ValueDocumentation = valueDocumentation;
    }

    /// <summary>
    /// Describes the function for documentation, with its parameters in braces, e.g. <c>bit 7 of {value} is set</c>. For a
    /// boolean function it is the condition the function tests; otherwise it is the value the function returns. <c>null</c> if
    /// the function is not documented.
    /// </summary>
    public string? Documentation { get; }

    /// <summary>
    /// Describes the condition a boolean function tests when it does not hold, e.g. <c>bit 7 of {value} is reset</c>.
    /// </summary>
    public string? NotDocumentation { get; }

    /// <summary>
    /// Describes the value of a boolean function when it is used as a number, e.g. <c>the half carry</c>.
    /// </summary>
    public string? ValueDocumentation { get; }

    public Expression Expression => expression ?? throw new InvalidOperationException($"{nameof(Expression)} not set.");

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)} => {Expression}";

    public static IReadOnlyDictionary<string, UserDefinedFunction> CreateDeclarations(IReadOnlyList<FunctionYaml> yamls) =>
        yamls.Select(
                yaml => new UserDefinedFunction(
                    yaml.Name,
                    YamlSerializer.Deserialize<DataType>(Encoding.UTF8.GetBytes(yaml.Type)),
                    yaml.Parameters,
                    yaml.Documentation,
                    yaml.NotDocumentation,
                    yaml.ValueDocumentation))
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