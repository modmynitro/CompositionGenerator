using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Modmynitro.SourceGenerators.Composition.Attributes;
using Modmynitro.SourceGenerators.Composition.Extensions;

namespace Modmynitro.SourceGenerators.Composition.SourceGenerators;

/// <summary>
/// https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
/// </summary>
[Generator]
public class CompositionSourceGenerator : IIncrementalGenerator
{
    private static readonly string AttributeName = typeof(CompositionAttribute).FullName ?? string.Empty;
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclaration = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) =>
                    GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        // Combine the selected enums with the `Compilation`
        var compilationAndEnums
            = context.CompilationProvider.Combine(classDeclaration.Collect());

        context.RegisterSourceOutput(compilationAndEnums,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    public static IEnumerable<INamedTypeSymbol> GetCompositions(IFieldSymbol fieldSymbol)
    {
        // loop through all the attributes on the field
        foreach (var attribute in fieldSymbol.GetAttributes())
        {
            var attributeSymbol = attribute.AttributeClass;

            var fullName = attributeSymbol?.ConstructedFrom.ToDisplayString();
            
            if (fullName == AttributeName && attribute.ConstructorArguments[0] is { Value: INamedTypeSymbol typeSymbol })
            {
                yield return typeSymbol;
            }
        }
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        if (node is FieldDeclarationSyntax fieldDeclarationSyntax)
            return fieldDeclarationSyntax.AttributeLists.Count > 0;

        return false;
    }

    private static Target? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var fieldDeclarationSyntax = (FieldDeclarationSyntax)context.Node;
        var compositions = new List<INamedTypeSymbol>();

        if (fieldDeclarationSyntax.Declaration.Variables.Count > 1)
            return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(fieldDeclarationSyntax.Declaration.Variables[0]);

        if (symbol is not IFieldSymbol fieldSymbol)
            return null;

        compositions.AddRange(GetCompositions(fieldSymbol).Distinct(SymbolEqualityComparer<INamedTypeSymbol>.Default));

        if (compositions.Count == 0)
            return null;

        return new(
            fieldDeclarationSyntax,
            compositions);
    }

    private static void Execute(
        Compilation compilation,
        IEnumerable<Target> sourceRight,
        SourceProductionContext spc)
    {
        foreach (var target in sourceRight.WhereNotNull())
        {
            var classDeclaration = target.FieldDeclaration.TryGetParent<ClassDeclarationSyntax>();

            if (classDeclaration is null || !classDeclaration.IsPartial())
                continue;

            var classSymbol = compilation.GetSemanticModel(classDeclaration.SyntaxTree.GetRoot().SyntaxTree, true)
                .GetDeclaredSymbol(classDeclaration);

            if (classSymbol is null)
                continue;

            var sb = new CodeGeneratorStringBuilder(compilation);

            foreach (var interfaceSymbol in target.Compositions)
            {
                var usedInterfaceSymbol = interfaceSymbol;

                if (interfaceSymbol.IsGenericType)
                {
                    var fieldSymbol = compilation.GetSemanticModel(target.FieldDeclaration.SyntaxTree)
                        .GetDeclaredSymbol(target.FieldDeclaration.Declaration.Variables[0]) as IFieldSymbol;

                    if (fieldSymbol is null)
                        continue;
                    
                    var unbound = interfaceSymbol.ConstructUnboundGenericType();

                    usedInterfaceSymbol = ((INamedTypeSymbol)fieldSymbol.Type).Interfaces
                        .Select(i => (Interface: i, unbound: i.ConstructUnboundGenericType()))
                        .Single(t => SymbolEqualityComparer<INamedTypeSymbol>.Default.Equals(t.unbound, unbound))
                        .Interface;
                }

                using (sb.CreateClassPart(classSymbol, usedInterfaceSymbol))
                {
                    CreateDelegateImplementations(sb, target.FieldDeclaration, usedInterfaceSymbol);
                }

                var code = sb.ToString();
                spc.AddSource($"{classSymbol.Name}.{interfaceSymbol.Name}", code);
            }
        }
    }

    private static void CreateDelegateImplementations(
        CodeGeneratorStringBuilder stringBuilder,
        FieldDeclarationSyntax fieldDeclaration,
        ITypeSymbol interfaceSymbol)
    {
        foreach (var property in interfaceSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            stringBuilder.AppendDelegateImplementation(fieldDeclaration, property);
        }

        foreach (var method in interfaceSymbol.GetMembers().OfType<IMethodSymbol>()
                     .Where(m => m.MethodKind is not (MethodKind.PropertyGet or MethodKind.PropertySet)))
        {
            stringBuilder.AppendDelegateImplementation(fieldDeclaration, method);
        }

        foreach (var @interface in interfaceSymbol.Interfaces)
        {
            CreateDelegateImplementations(stringBuilder, fieldDeclaration, @interface);
        }
    }

    private sealed record Target(
        FieldDeclarationSyntax FieldDeclaration,
        IReadOnlyList<INamedTypeSymbol> Compositions);
}