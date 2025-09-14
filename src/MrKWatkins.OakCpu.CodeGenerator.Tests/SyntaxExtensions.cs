using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

internal static class SyntaxExtensions
{
    [Pure]
    internal static string ToNormalizedString(this SyntaxNode node) =>
        node.NormalizeWhitespace().ToFullString();
}