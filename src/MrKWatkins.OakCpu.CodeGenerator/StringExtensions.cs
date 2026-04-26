using System.Globalization;
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
        var first = true;
        foreach (var segment in snakeCase.Split('_'))
        {
            if (segment.Length == 0)
            {
                continue;
            }

            if (string.Equals(segment, "io", StringComparison.OrdinalIgnoreCase))
            {
                result.Append(lower && first ? "io" : "IO");
            }
            else if (lower && first)
            {
                result.Append(segment.ToLower(CultureInfo.InvariantCulture));
            }
            else
            {
                result.Append(char.ToUpper(segment[0], CultureInfo.InvariantCulture));
                result.Append(lower ? segment[1..].ToLower(CultureInfo.InvariantCulture) : segment[1..]);
            }

            first = false;
        }

        return result.ToString();
    }
}