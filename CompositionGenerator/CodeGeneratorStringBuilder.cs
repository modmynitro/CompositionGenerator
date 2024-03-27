using Microsoft.CodeAnalysis;

namespace Modmynitro.SourceGenerators.Composition;

public class CodeGeneratorStringBuilder
{
    public Compilation Compilation { get; }
    private readonly PrettyCode.StringBuilder _stringBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGeneratorStringBuilder" /> class.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    public CodeGeneratorStringBuilder(Compilation compilation)
#pragma warning disable RS1035
        : this(compilation, new(), 4, ' ', Environment.NewLine, 0)
#pragma warning restore RS1035
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="compilation">The compilation used to resolve types.</param>
    /// <param name="buffer">The buffer used to store the generated strings.</param>
    /// <param name="spacesPerIndentation">The number of spaces per  indentation..</param>
    /// <param name="space">The character to be used as space.</param>
    /// <param name="newLine">The sequence of character representing a new line.</param>
    /// <param name="indentation">The initial indentation level.</param>
    public CodeGeneratorStringBuilder(
        Compilation compilation,
        System.Text.StringBuilder buffer,
        int spacesPerIndentation,
        char space,
        string newLine,
        int indentation)
    {
        Compilation = compilation;
        _stringBuilder = new(buffer, spacesPerIndentation, space, newLine, indentation);
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.AppendLines"/>
    public CodeGeneratorStringBuilder AppendLines(IEnumerable<string> lines)
    {
        _stringBuilder.AppendLines(lines);
        return this;
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.AppendLine"/>
    public CodeGeneratorStringBuilder AppendLine(string line)
    {
        _stringBuilder.AppendLine(line);
        return this;
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.AppendEmptyLine"/>
    public CodeGeneratorStringBuilder AppendEmptyLine()
    {
        _stringBuilder.AppendEmptyLine();
        return this;
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.CurlyBracesBlock"/>
    public IDisposable CurlyBracesBlock(bool trailingSemicolon = false, bool indent = true)
    {
        return _stringBuilder.CurlyBracesBlock(trailingSemicolon, indent);
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.Indent"/>
    public IDisposable Indent() => _stringBuilder.Indent();

    /// <inheritdoc cref="PrettyCode.StringBuilder.NullableDirective"/>
    public IDisposable NullableDirective(bool enable = true)
    {
        return _stringBuilder.NullableDirective(enable); 
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.PragmaWarningDirective"/>
    public IDisposable PragmaWarningDirective(params string[] warnings)
    {
        return _stringBuilder.PragmaWarningDirective(warnings);
    }

    /// <inheritdoc cref="PrettyCode.StringBuilder.RegionDirective"/>
    public IDisposable RegionDirective(string? regionName = null)
    {
        return _stringBuilder.RegionDirective(regionName);
    }

    public override string ToString() => _stringBuilder.ToString();
}