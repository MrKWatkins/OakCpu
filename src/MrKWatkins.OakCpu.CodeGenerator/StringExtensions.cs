using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator;

public static class StringExtensions
{
    [Pure]
    public static string ToUpperCamelCaseFromSnakeCase(this string snakeCase) => ToCamelCaseFromSnakeCase(snakeCase, false);

    [Pure]
    public static string ToLowerCamelCaseFromSnakeCase(this string snakeCase) => ToCamelCaseFromSnakeCase(snakeCase, true);

    [Pure]
    private static string ToCamelCaseFromSnakeCase(this string snakeCase, bool lower)
    {
        var result = new StringBuilder();
        var needsUpper = !lower;
        foreach (var character in snakeCase)
        {
            if (character == '_')
            {
                needsUpper = true;
                continue;
            }
            if (needsUpper)
            {
                result.Append(char.ToUpper(character));
                needsUpper = false;
            }
            else if (lower)
            {
                result.Append(char.ToLower(character));
            }
            else
            {
                result.Append(character);
            }
        }
        return result.ToString();
    }
}