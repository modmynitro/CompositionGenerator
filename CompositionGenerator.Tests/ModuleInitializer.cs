using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace CompositionGeneratorTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        
        MoDMyNitro.SourceGenerators.Composition.TestHelper.EnsureLoaded();
        MoDMyNitro.SourceGenerators.Composition.Attributes.TestHelper.EnsureLoaded();
    }
}