using Microsoft.CodeAnalysis;

namespace Modmynitro.SourceGenerators.Composition;

public class SymbolEqualityComparer<TSymbol> : IEqualityComparer<TSymbol?>
    where TSymbol : ISymbol
{
    /// <summary>
    /// Compares two <see cref="ISymbol"/> instances based on the default comparison rules, equivalent to calling <see cref="IEquatable{ISymbol}.Equals(ISymbol)"/>.
    /// </summary>
    /// <remarks>
    /// Comparing <c>string</c> and <c>string?</c> will return equal. Use <see cref="IncludeNullability"/> if you don't want them to be considered equal.
    /// </remarks>
    public static readonly SymbolEqualityComparer<TSymbol> Default = new(false);
    
    /// <summary>
    /// Compares  two <see cref="ISymbol"/> instances, considering that a reference type and the same nullable reference type are not equal.
    /// </summary>
    /// <remarks>
    /// Comparing <c>string</c> and <c>string?</c> will not return equal. Use <see cref="Default"/> if you want them to be considered equal.
    /// </remarks>
    public static readonly SymbolEqualityComparer<TSymbol> IncludeNullability = new(true);

    private readonly SymbolEqualityComparer _implementation;

    public bool Equals(TSymbol? x, TSymbol? y)
    {
        return _implementation.Equals(x, y);
    }

    public int GetHashCode(TSymbol? obj)
    {
        return _implementation.GetHashCode(obj);
    }

    private SymbolEqualityComparer(bool includeNullability)
    {
        _implementation = includeNullability
            ? SymbolEqualityComparer.IncludeNullability
            : SymbolEqualityComparer.Default;
    }
}