namespace MoDMyNitro.SourceGenerators.Composition.Extensions;

internal static class EnumerableExtensions
{
    /// <summary>
    /// Filters out all <see langword="null"/> elements from the collection.
    /// </summary>
    /// <param name="source">An <see cref="System.Collections.Generic.IEnumerable{TSource}" /> to filter.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <returns>An <see cref="System.Collections.Generic.IEnumerable{TSource}" /> with all elements which are not <see langword="null"/>.</returns>
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> source)
    {
        return source.Where(element => element is not null).Cast<TSource>();
    }
}