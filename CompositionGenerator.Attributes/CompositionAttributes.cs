namespace MoDMyNitro.SourceGenerators.Composition.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class CompositionAttribute : Attribute
{
    public CompositionAttribute(Type @interface)
    {
        Interface = @interface;
    }

    public Type Interface { get; }
}