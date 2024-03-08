using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using MoDMyNitro.SourceGenerators.Composition.Diagnostics;
using MoDMyNitro.SourceGenerators.Composition.SourceGenerators;

namespace MoDMyNitro.SourceGenerators.Composition.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CompositionAttributeAnalyzer : DiagnosticAnalyzer
{
    private ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
        
        throw new NotImplementedException();
    }

    private void AnalyzeSymbol(SymbolAnalysisContext obj)
    {
        var fieldSymbol = (IFieldSymbol)obj.Symbol;

        var compositions = CompositionSourceGenerator.GetCompositions(fieldSymbol).ToList();
        
        if (compositions is { Count: < 1 })
            return;

        if (fieldSymbol.DeclaringSyntaxReferences
            .OfType<FieldDeclarationSyntax>()
            .Any(d => d.Declaration.Variables.Count > 1))
            obj.ReportDiagnostic(
                Diagnostic.Create(Descriptors.FieldDeclarationShouldOnlyDeclareOneField, fieldSymbol.Locations[0]));
        
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

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, new() { Descriptors.FieldDeclarationShouldOnlyDeclareOneField, Descriptors.FieldShouldImplementSpecifiedInterface });

            return _supportedDiagnostics;
        }
    }
}