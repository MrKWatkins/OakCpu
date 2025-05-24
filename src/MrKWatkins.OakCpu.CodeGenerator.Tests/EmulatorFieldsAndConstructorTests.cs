using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class EmulatorFieldsAndConstructorTests : TextFixture
{
    [Test]
    public async Task Create()
    {
        var input = await LoadTestDataAsync();

        var classDefinition = EmulatorFieldsAndConstructor.Instance.Generate(input);
    }
}