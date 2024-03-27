using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Modmynitro.SourceGenerators.Composition.Extensions;

public static class ClassDeclarationSyntaxExtensions
{
    /// <summary>
    /// Checks if the class is marked as partial.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class declaration.</param>
    /// <returns><see langword="true"/> if marked as partial, otherwise <see langword="false"/>.</returns>
    public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    /// <summary>
    /// Gets the namespace of the class.
    /// </summary>
    /// <param name="classDeclarationSyntax">The class declaration.</param>
    /// <returns>The namespace or <see langword="null"/> if no namespace can be found.</returns>
    public static string? GetNameSpace(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        var parent = classDeclarationSyntax.TryGetParent<NamespaceDeclarationSyntax>();

        return parent?.Name.ToString();
    }
}