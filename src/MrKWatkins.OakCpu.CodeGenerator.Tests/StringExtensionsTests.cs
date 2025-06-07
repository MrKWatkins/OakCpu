namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class StringExtensionsTests
{
    [TestCase("snake", "Snake")]
    [TestCase("snake_case", "SnakeCase")]
    [TestCase("snake_case_case", "SnakeCaseCase")]
    public void ToUpperCamelCaseFromSnakeCase(string snakeCase, string expected) => snakeCase.ToUpperCamelCaseFromSnakeCase().Should().Be(expected);
}