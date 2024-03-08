using Microsoft.CodeAnalysis;

namespace MoDMyNitro.SourceGenerators.Composition.Diagnostics;

public static class Descriptors
{
    public static readonly DiagnosticDescriptor FieldDeclarationShouldOnlyDeclareOneField = new(
        "CG0001",
        "Declaration should only declare a single field",
        "Declaration should only declare a single field",
        "CompositionGenerator",
        DiagnosticSeverity.Error,
        true);
    
    public static readonly DiagnosticDescriptor FieldShouldImplementSpecifiedInterface = new(
        "CG0002",
        "Field should implement specified interface",
        "Field '{0}' should implement specified interface '{1}'",
        "CompositionGenerator",
        DiagnosticSeverity.Error,
        true);
}