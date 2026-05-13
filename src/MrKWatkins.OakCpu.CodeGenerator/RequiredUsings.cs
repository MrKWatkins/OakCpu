using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

internal sealed class RequiredUsings
{
    private readonly HashSet<string> namespaces = new(StringComparer.Ordinal);

    internal int Count => namespaces.Count;

    internal void Add(string @namespace) => namespaces.Add(@namespace);

    internal void Add(Type type) => Add(type.Namespace!);

    [Pure]
    internal bool Contains(string @namespace) => namespaces.Contains(@namespace);

    [Pure]
    internal IReadOnlyList<UsingDirectiveSyntax> CreateUsingDirectives() => namespaces
        .OrderBy(@namespace => @namespace, StringComparer.Ordinal)
        .Select(@namespace => UsingDirective(ParseName(@namespace)))
        .ToList();
}