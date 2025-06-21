namespace MrKWatkins.OakCpu.Z80.TestSuites;

internal static class EnumerableExtensions
{
    [Pure]
    internal static IEnumerable<(T? Previous, T Current, T? Next)> EnumerateWindow<T>(this IEnumerable<T> source)
        where T: struct
    {
        using var enumerator = source.GetEnumerator();
        enumerator.MoveNext();

        T? previous = null;
        var current = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (previous, current, enumerator.Current);
            previous = current;
            current = enumerator.Current;
        }
        yield return (previous, current, null);
    }
}