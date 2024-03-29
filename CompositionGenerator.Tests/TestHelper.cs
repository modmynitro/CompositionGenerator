﻿using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CompositionGeneratorTests;

public static class TestHelper
{
    public static Task Verify<TGenerator>(string source)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        var compilation = CreateCompilation(source);
        
        // Create an instance of our EnumGenerator incremental source generator
        var generator = new TGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);

        var diagnostics = newCompilation.GetDiagnostics();

        if (diagnostics.Length != 0)
        {
            diagnostics.Should().BeEmpty();
            var diagnosticString = string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString()));
            throw new InvalidOperationException(diagnosticString);
        }
        
        return Verifier.Verify(driver.GetRunResult());
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        //Load project libraries
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.Location)
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => !s.Contains("nunit"))
            .Select(s => MetadataReference.CreateFromFile(s))
            .ToList();

        // Create a Roslyn compilation for the syntax tree.
        return CSharpCompilation.Create(
            assemblyName: "Tests",
            references: references,
            syntaxTrees: new[] { syntaxTree },
            options: new(OutputKind.DynamicallyLinkedLibrary));
    }
}