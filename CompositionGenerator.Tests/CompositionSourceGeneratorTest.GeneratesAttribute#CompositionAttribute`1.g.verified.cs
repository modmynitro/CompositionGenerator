//HintName: CompositionAttribute`1.g.cs
using System;

namespace CompositionGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class CompositionAttribute : Attribute
    {
        public CompositionAttribute(Type @interface)
        {
            Interface = @interface;
        }
        
        public Type Interface { get; }
    }
}