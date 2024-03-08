using Microsoft.CodeAnalysis;

namespace MoDMyNitro.SourceGenerators.Composition.Extensions;

public static class AccessibilityExtensions
{
    public static string ToDisplayString(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null)
        };
    }
}