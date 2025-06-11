using System.Text;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using MrKWatkins.OakCpu.CodeGenerator.Language.Parsing;
using MrKWatkins.OakCpu.CodeGenerator.Yaml;
using VYaml.Serialization;

namespace MrKWatkins.OakCpu.CodeGenerator.Definitions;

public sealed class UserDefinedFunction : Function
{
    private UserDefinedFunction(string name, DataType type, IReadOnlyList<string> parameters)
        : base(name, type, parameters)
    {
    }

    public Expression Expression { get; private set; } = null!;

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)} => {Expression}";

    public static void AddToConfiguration(Configuration configuration, IReadOnlyList<FunctionYaml> yamls)
    {
        // Two pass as they might reference each other.
        var functions = yamls.Select(yaml => (Yaml: yaml, Function: new UserDefinedFunction(yaml.Name, YamlSerializer.Deserialize<DataType>(Encoding.UTF8.GetBytes(yaml.Type)), yaml.Parameters))).ToList();

        configuration.UserDefinedFunctions = functions.ToDictionary(u => u.Function.Name, u => u.Function);

        var context = new ParserContext(configuration);
        foreach (var (yaml, function) in functions)
        {
            function.Expression = Parser.ParseExpression(context.WithArguments(yaml.Parameters), yaml.Expression);
        }
    }
}