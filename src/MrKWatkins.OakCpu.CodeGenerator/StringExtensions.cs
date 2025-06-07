using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator;

public static class StringExtensions
{
    [Pure]
    public static string ToUpperCamelCaseFromSnakeCase(this string snakeCase)
    {
        var result = new StringBuilder();
        var needsUpper = true;
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
            else
            {
                result.Append(character);
            }
        }
        return result.ToString();
    }
}