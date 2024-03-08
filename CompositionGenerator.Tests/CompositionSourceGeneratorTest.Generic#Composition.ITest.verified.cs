//HintName: Composition.ITest.cs

partial class Composition<TComp> : ITest<TComp>
{
    public TComp Test { get => _composition.Test; }
}