using System.Collections.Immutable;
using CompositionGeneratorTests;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Modmynitro.SourceGenerators.Composition.Analyzers;

public class MyAnalyzerTests
{
    [Test]
    public void TestAnalyzer()
    {
        // Arrange
        var source =
            """
            using Modmynitro.SourceGenerators.Composition.Attributes;

            public interface ITest
            {
                int Test();
            }

            public class TestImplementation : ITest
            {
                public int Test()
                {
                    return 4711;
                }
            }

            public class Composition
            {
                [CompositionAttribute(typeof(ITest))]
                private readonly TestImplementation _composition = new(), _composition2 = new();
            }
            """;

        // Act
        var diagnostics = TestHelper.GetDiagnostics<CompositionAttributeAnalyzer>(source);

        // Assert
        diagnostics.Should().NotBeEmpty();
    }

    private static Document CreateDocument1(string code)
    {
        var projectId = ProjectId.CreateNewId();
        var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, "TestProject", "TestAssembly", LanguageNames.CSharp);
        var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution.AddProject(projectInfo);
        var documentId = DocumentId.CreateNewId(projectId);
        solution = solution.AddDocument(documentId, "TestDocument", SourceText.From(code));
        return solution.GetDocument(documentId);
    }

    private static Diagnostic[] GetDiagnostics1(DiagnosticAnalyzer analyzer, Document document)
    {
        var compilation = document.Project.GetCompilationAsync().Result;
        var diagnostics = Diagnostic.Create(analyzer.SupportedDiagnostics[0], Location.None);
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer));
        var diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
        return diags.ToArray();
    }
}