using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

internal sealed class RequiredUsings
{
    private readonly HashSet<string> namespaces = new(StringComparer.Ordinal);

    public int Count => namespaces.Count;

    public void Add(string @namespace) => namespaces.Add(@namespace);

    public void Add(Type type) => Add(type.Namespace!);

    [Pure]
    public bool Contains(string @namespace) => namespaces.Contains(@namespace);

    [Pure]
    public IReadOnlyList<UsingDirectiveSyntax> CreateUsingDirectives() => namespaces
        .OrderBy(@namespace => @namespace, StringComparer.Ordinal)
        .Select(@namespace => UsingDirective(ParseName(@namespace)))
        .ToList();
}