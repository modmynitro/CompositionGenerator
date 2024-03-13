using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace CompositionGeneratorTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        
        Modmynitro.SourceGenerators.Composition.TestHelper.EnsureLoaded();
        Modmynitro.SourceGenerators.Composition.Attributes.TestHelper.EnsureLoaded();
    }
}