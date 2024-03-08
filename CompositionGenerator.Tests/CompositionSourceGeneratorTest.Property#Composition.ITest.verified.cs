//HintName: Composition.ITest.cs

partial class Composition : ITest
{
    public int Test { get => _composition.Test; set => _composition.Test = value; }
}