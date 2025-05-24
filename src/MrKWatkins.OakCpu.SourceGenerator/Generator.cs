using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MrKWatkins.OakCpu.CodeGenerator;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using SGF;

namespace MrKWatkins.OakCpu.SourceGenerator;

[IncrementalGenerator]
public sealed class Generator() : IncrementalGenerator(nameof(Generator))
{
    public override void OnInitialize(SgfInitializationContext context)
    {
        var rootNamespace = context
            .AnalyzerConfigOptionsProvider
            .Select((c, _) => c.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace) ? nameSpace : null);

        var yamls = context
            .AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            .Collect();

        var generatorInput = rootNamespace.Combine(yamls).Select((x, _) => GeneratorInput.Create(x.Left, x.Right));

        context.RegisterSourceOutput(generatorInput, GenerateCode);
    }

    private void GenerateCode(SgfSourceProductionContext context, GeneratorInput input)
    {
        foreach (var generator in ClassGenerator.AllGenerators)
        {
            try
            {
                var compilationUnit = generator.Generate(input);
                context.AddSource(generator.FileName, SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error generating {generator.FileName}.");
            }
        }
    }
}