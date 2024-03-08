using Microsoft.CodeAnalysis;

namespace CompositionGeneratorTests;

public sealed record RunResultWithIgnoreList
{
    public required GeneratorDriverRunResult Result { get; init; }
    public IReadOnlyList<string> IgnoredFiles { get; init; } = Array.Empty<string>();
}