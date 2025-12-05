using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Console;

public static class Generator
{
    public static async Task GenerateAsync(string cpu, string project)
    {
        var solutionDirectory = FindSolutionDirectory();
        var definitionsDirectory = GetChildDirectory(solutionDirectory, "../", "definitions", cpu);

        var generatorContext = await GeneratorContext.CreateAsync(project, definitionsDirectory);

        var projectDirectory = GetChildDirectory(solutionDirectory, project);

        foreach (var generator in TypeGenerator.AllGenerators)
        {
            await GenerateAsync(projectDirectory, generatorContext, generator);
        }
    }

    private static async Task GenerateAsync(DirectoryInfo projectDirectory, GeneratorContext generatorContext, TypeGenerator generator)
    {
        var fileName = generator.GetFileName(generatorContext);
        try
        {
            var source = generator.Generate(generatorContext);
            var outputPath = Path.Join(projectDirectory.FullName, fileName);
            await using var writer = File.CreateText(outputPath);
            await writer.WriteLineAsync(source);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Exception generating {fileName}.", exception);
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