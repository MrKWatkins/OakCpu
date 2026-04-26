namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class StringExtensionsTests
{
    [TestCase("snake", "Snake")]
    [TestCase("snake_case", "SnakeCase")]
    [TestCase("snake_case_case", "SnakeCaseCase")]
    [TestCase("SNAKE_CASE_CASE", "SNAKECASECASE")]
    [TestCase("io_read", "IORead")]
    [TestCase("memory_io_read", "MemoryIORead")]
    public void ToUpperCamelCaseFromSnakeCase(string snakeCase, string expected) => snakeCase.ToUpperCamelCaseFromSnakeCase().Should().Equal(expected);

    [TestCase("snake", "snake")]
    [TestCase("snake_case", "snakeCase")]
    [TestCase("snake_case_case", "snakeCaseCase")]
    [TestCase("SNAKE_CASE_CASE", "snakeCaseCase")]
    [TestCase("io_read", "ioRead")]
    [TestCase("memory_io_read", "memoryIORead")]
    public void ToLowerCamelCaseFromSnakeCase(string snakeCase, string expected) => snakeCase.ToLowerCamelCaseFromSnakeCase().Should().Equal(expected);
}