namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class StringExtensionsTests
{
    [TestCase("snake", "Snake")]
    [TestCase("snake_case", "SnakeCase")]
    [TestCase("snake_case_case", "SnakeCaseCase")]
    [TestCase("SNAKE_CASE_CASE", "SNAKECASECASE")]
    public void ToUpperCamelCaseFromSnakeCase(string snakeCase, string expected) => snakeCase.ToUpperCamelCaseFromSnakeCase().Should().Be(expected);

    [TestCase("snake", "snake")]
    [TestCase("snake_case", "snakeCase")]
    [TestCase("snake_case_case", "snakeCaseCase")]
    [TestCase("SNAKE_CASE_CASE", "snakeCaseCase")]
    public void ToLowerCamelCaseFromSnakeCase(string snakeCase, string expected) => snakeCase.ToLowerCamelCaseFromSnakeCase().Should().Be(expected);
}