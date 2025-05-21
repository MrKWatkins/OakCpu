namespace MrKWatkins.OakCpu.SourceGenerator.Tests.Yaml;

public abstract class YamlTextFixture
{
    [Pure]
    [MustDisposeResource]
    protected static Stream LoadTestData(string name) =>
        typeof(YamlTextFixture).Assembly.GetManifestResourceStream(typeof(YamlTextFixture), $"TestData.{name}")
        ?? throw new InvalidOperationException($"Could not find embedded resource {name}.");
}