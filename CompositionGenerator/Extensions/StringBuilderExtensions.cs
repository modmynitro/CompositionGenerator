using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MoDMyNitro.SourceGenerators.Composition.Extensions;

public static class StringBuilderExtensions
{
    public static void AddUsing(this PrettyCode.StringBuilder stringBuilder, string @namespace)
    {
        stringBuilder.AppendLine($"using {@namespace};");
    }
    
    public static IDisposable CreateClassPart(this CodeGeneratorStringBuilder stringBuilder, INamedTypeSymbol classSymbol, params INamedTypeSymbol[] interfaceSymbols)
    {
        var declaration = interfaceSymbols.Length > 0 ? 
            $"partial class {classSymbol.ToDisplayString()} : {string.Join(", ", interfaceSymbols.Select(i => i.ToDisplayString()))}" :
            $"partial class {classSymbol.ToDisplayString()}";
        
        if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            stringBuilder.AppendLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString()};");
            
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(declaration);
        
        return stringBuilder.CurlyBracesBlock();
    }

    public static void AppendDelegateImplementation(this CodeGeneratorStringBuilder stringBuilder, FieldDeclarationSyntax fieldDeclaration, IPropertySymbol propertySymbol)
    {
        if (propertySymbol.GetMethod is not null && propertySymbol.SetMethod is not null)
        {
            var declaration = $"public {propertySymbol.Type.ToDisplayString()} {propertySymbol.Name}";
            var body = $"get => {fieldDeclaration.Declaration.Variables[0].Identifier.Text}.{propertySymbol.Name}; set => {fieldDeclaration.Declaration.Variables[0].Identifier.Text}.{propertySymbol.Name} = value;";
            
            stringBuilder.AppendLine($"{declaration} {{ {body} }}");
        }
        else if (propertySymbol.GetMethod is not null)
        {
            var declaration = $"public {propertySymbol.Type.ToDisplayString()} {propertySymbol.Name}";
            var body = $"get => {fieldDeclaration.Declaration.Variables[0].Identifier.Text}.{propertySymbol.Name};";
            
            stringBuilder.AppendLine($"{declaration} {{ {body} }}");
        }
        else if (propertySymbol.SetMethod is not null)
        {
            var declaration = $"public {propertySymbol.Type.ToDisplayString()} {propertySymbol.Name}";
            var body = $"set => {fieldDeclaration.Declaration.Variables[0].Identifier.Text}.{propertySymbol.Name} = value;";
            
            stringBuilder.AppendLine($"{declaration} {{ {body} }}");
        }
    }
    
    public static void AppendDelegateImplementation(this CodeGeneratorStringBuilder stringBuilder, FieldDeclarationSyntax fieldDeclaration, IMethodSymbol methodSymbol)
    {
        var declaration = $"public {methodSymbol.ReturnType.ToDisplayString()} {methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"))})";
        var body = $"{fieldDeclaration.Declaration.Variables[0].Identifier.Text}.{methodSymbol.Name}({string.Join(", ", methodSymbol.Parameters.Select(p => p.Name))});";
        
        stringBuilder.AppendLine($"{declaration} => {body}");
    }
}