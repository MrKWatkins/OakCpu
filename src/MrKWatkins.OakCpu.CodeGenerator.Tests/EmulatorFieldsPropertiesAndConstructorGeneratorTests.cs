using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class EmulatorFieldsPropertiesAndConstructorGeneratorTests : TextFixture
{
    [Test]
    public async Task Create()
    {
        var input = await LoadTestDataAsync();

        var classDefinition = EmulatorFieldsPropertiesAndConstructorGenerator.Instance.Generate(input);
    }
}