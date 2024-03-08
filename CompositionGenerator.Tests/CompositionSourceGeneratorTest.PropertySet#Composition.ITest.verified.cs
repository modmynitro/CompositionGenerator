//HintName: Composition.ITest.cs

partial class Composition : ITest
{
    public int Test { set => _composition.Test = value; }
}