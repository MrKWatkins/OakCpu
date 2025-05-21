using System.Data;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MrKWatkins.OakCpu.SourceGenerator;

[Generator]
public sealed class RegistersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //Thread.Sleep(10000);
        var registersYaml = context
            .AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            .Select<AdditionalText, string>((file, _) => Path.GetFullPath(file.Path));

        context.RegisterSourceOutput(registersYaml, GenerateCode);
    }

    private void GenerateCode(SourceProductionContext context, string path)
    {
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("OC0001", "Path", path, "Files", DiagnosticSeverity.Warning, true), Location.None));
    }
}