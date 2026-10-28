using MrKWatkins.OakCpu.Definitions;

namespace MrKWatkins.OakCpu.DocumentationGenerator.Console;

public static class Generator
{
    public static async Task GenerateAsync(string cpu)
    {
        System.Console.WriteLine($"Generating documentation for {cpu}...");

        try
        {
            var solutionDirectory = FindSolutionDirectory();
            var definitionsDirectory = GetChildDirectory(solutionDirectory, "../", "definitions", cpu);
            var outputDirectory = GetChildDirectory(solutionDirectory, "../", "docs", "instructions", cpu);

            var model = await CpuModel.LoadAsync(definitionsDirectory);
            var documents = DocumentationGenerator.Generate(model);

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            foreach (var document in documents)
            {
                var file = new FileInfo(Path.Join(outputDirectory.FullName, document.Path));
                file.Directory!.Create();
                await File.WriteAllTextAsync(file.FullName, document.Markdown);
            }

            System.Console.WriteLine($"Generation of {documents.Count} documents for {cpu} in {outputDirectory} complete.");
        }
        catch (Exception exception)
        {
            var foregroundColour = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Exception generating documentation for {cpu}: {exception}");
            System.Console.ForegroundColor = foregroundColour;
            throw;
        }
    }

    [Pure]
    private static DirectoryInfo GetChildDirectory(DirectoryInfo root, params string[] children) => new(Path.Join(children.Prepend(root.FullName).ToArray()));

    [Pure]
    private static DirectoryInfo FindSolutionDirectory()
    {
        var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        while (true)
        {
            if (currentDirectory.EnumerateFiles("OakCpu.slnx").Any())
            {
                return currentDirectory;
            }

            currentDirectory = currentDirectory.Parent ?? throw new InvalidOperationException("Cannot find solution directory.");
        }
    }
}