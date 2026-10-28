namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class VerifyChecksTests
{
    [Test]
    public Task RunAsync() => VerifyChecks.Run();
}