using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Modmynitro.SourceGenerators.Composition.Diagnostics;
using Modmynitro.SourceGenerators.Composition.Extensions;
using Modmynitro.SourceGenerators.Composition.SourceGenerators;

namespace Modmynitro.SourceGenerators.Composition.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CompositionAttributeAnalyzer : DiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> StaticSupportedDiagnostics =
    [
        Descriptors.FieldDeclarationShouldOnlyDeclareOneField,
        Descriptors.FieldShouldImplementSpecifiedInterface
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext obj)
    {
        var fieldSymbol = (IFieldSymbol)obj.Symbol;

        var compositions = CompositionSourceGenerator.GetCompositions(fieldSymbol).ToList();

        if (compositions.Count < 1)
            return;

        if (fieldSymbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .Select(n => n.TryGetParent<FieldDeclarationSyntax>())
            .WhereNotNull()
            .Any(d => d.Declaration.Variables.Count > 1))
            obj.ReportDiagnostic(Diagnostic.Create(Descriptors.FieldDeclarationShouldOnlyDeclareOneField, fieldSymbol.Locations[0]));

        var interfaces = compositions.SelectMany(c => c.Interfaces).Distinct(SymbolEqualityComparer<INamedTypeSymbol>.Default).ToList();

        var missingInterfaces = interfaces.Except(fieldSymbol.Type.AllInterfaces, SymbolEqualityComparer<INamedTypeSymbol>.Default).ToList();

        if (missingInterfaces.Count is 0)
            return;

        foreach (var missingInterface in missingInterfaces)
        {
            obj.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptors.FieldShouldImplementSpecifiedInterface,
                    fieldSymbol.Locations[0],
                    fieldSymbol.Name,
                    missingInterface.Name));
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => StaticSupportedDiagnostics;
}