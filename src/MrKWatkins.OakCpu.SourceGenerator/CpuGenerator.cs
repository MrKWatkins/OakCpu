using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using MrKWatkins.OakCpu.CodeGenerator;
using MrKWatkins.OakCpu.CodeGenerator.Generators;
using SGF;

namespace MrKWatkins.OakCpu.SourceGenerator;

[IncrementalGenerator]
public sealed class CpuGenerator() : IncrementalGenerator(nameof(CpuGenerator))
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
        try
        {
            AddSource(context, "FieldsAndConstructor", EmulatorFieldsAndConstructor.Instance.Generate(input));
        }
        catch (Exception exception)
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("OC0001", "Path", exception.ToString(), "Files", DiagnosticSeverity.Error, true), Location.None));
        }
    }

    private static void AddSource(SgfSourceProductionContext context, string name, CompilationUnitSyntax compilationUnit) =>
        context.AddSource($"Z80Emulator.{name}.g", SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
}