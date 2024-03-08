//HintName: Composition.ITest.cs

partial class Composition : ITest
{
    public int Test { get => _composition.Test; }
    public int TestBase { get => _composition.TestBase; }
}