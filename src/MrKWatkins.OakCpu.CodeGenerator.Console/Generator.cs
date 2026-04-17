using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Console;

public static class Generator
{
    public static async Task GenerateAsync(string cpu, string project)
    {
        System.Console.WriteLine($"Generating code for {cpu} in {project}...");

        try
        {
            var solutionDirectory = FindSolutionDirectory();
            var definitionsDirectory = GetChildDirectory(solutionDirectory, "../", "definitions", cpu);

            var generatorContext = await GeneratorContext.CreateAsync(project, definitionsDirectory);

            var projectDirectory = GetChildDirectory(solutionDirectory, project);

            foreach (var generator in TypeGenerator.AllGenerators)
            {
                await GenerateAsync(projectDirectory, generatorContext, generator);
            }

            System.Console.WriteLine($"Generation of code for {cpu} in {project} complete.");
        }
        catch (Exception exception)
        {
            var foregroundColour = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Exception generating code for {cpu} in {project}: {exception}");
            System.Console.ForegroundColor = foregroundColour;
            throw;
        }
    }

    private static async Task GenerateAsync(DirectoryInfo projectDirectory, GeneratorContext generatorContext, TypeGenerator generator)
    {
        System.Console.WriteLine($"Running {generator.GetType().Name}...");
        try
        {
            foreach (var generatedFile in generator.GenerateFiles(generatorContext))
            {
                var outputPath = Path.Join(projectDirectory.FullName, generatedFile.FileName);
                await using var writer = File.CreateText(outputPath);
                await writer.WriteLineAsync(generatedFile.Source);
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Exception generating output for {generator.GetType().Name}.", exception);
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
            if (currentDirectory.EnumerateFiles("OakCpu.sln").Any())
            {
                return currentDirectory;
            }

            currentDirectory = currentDirectory.Parent ?? throw new InvalidOperationException("Cannot find solution directory.");
        }
    }
}