using Microsoft.CodeAnalysis;

namespace MoDMyNitro.SourceGenerators.Composition.Extensions;

internal static class SyntaxNodeExtensions
{
    /// <summary>
    /// Tries to get the first parent with the specified type.
    /// </summary>
    /// <param name="node">The node of which to get the parent. Must not be <see langword="null"/>.</param>
    /// <typeparam name="TParent">Type of the parent to get.</typeparam>
    /// <returns>The parent or <see langword="null"/> if it can not be found.</returns>
    public static TParent? TryGetParent<TParent>(this SyntaxNode? node)
        where TParent : SyntaxNode
    {
        var parent = node?.Parent;

        while (parent != null)
        {
            if (parent is TParent parentNode)
                return parentNode;

            parent = parent.Parent;
        }

        return null;
    }
}