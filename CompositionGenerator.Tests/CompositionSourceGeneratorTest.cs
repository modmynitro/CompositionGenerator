using Modmynitro.SourceGenerators.Composition.SourceGenerators;

namespace CompositionGeneratorTests;

public class CompositionSourceGeneratorTest
{
    [Test]
    public Task SimpleInterface()
    {
        // The source code to test
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

            public partial class Composition
            {
                [CompositionAttribute(typeof(ITest))]
                private readonly TestImplementation _composition = new();
            }
            """;

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify<CompositionSourceGenerator>(source);
    }
    
    [Test]
    public Task Property()
    {
        // The source code to test
        var source =
            """
            using Modmynitro.SourceGenerators.Composition.Attributes;

            public interface ITest
            {
                int Test { get; set; }
            }

            public class TestImplementation : ITest
            {
                public int Test { get; set; }
            }

            public partial class Composition
            {
                [CompositionAttribute(typeof(ITest))]
                private readonly TestImplementation _composition = new();
            }
            """;

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify<CompositionSourceGenerator>(source);
    }
    
    [Test]
    public Task PropertySet()
    {
        // The source code to test
        var source =
            """
            using Modmynitro.SourceGenerators.Composition.Attributes;

            public interface ITest
            {
                int Test { set; }
            }

            public class TestImplementation : ITest
            {
                public int Test { private get; set; }
            }

            public partial class Composition
            {
                [CompositionAttribute(typeof(ITest))]
                private readonly TestImplementation _composition = new();
            }
            """;

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify<CompositionSourceGenerator>(source);
    }
    
    [Test]
    public Task PropertyGet()
    {
        // The source code to test
        var source =
            """
            using Modmynitro.SourceGenerators.Composition.Attributes;

            public interface ITest
            {
                int Test { get; }
            }

            public class TestImplementation : ITest
            {
                public int Test { get; }
            }

            public partial class Composition
            {
                [CompositionAttribute(typeof(ITest))]
                private readonly TestImplementation _composition = new();
            }
            """;

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify<CompositionSourceGenerator>(source);
    }
    
    [Test]
    public Task Inheritance()
    {
        // The source code to test
        var source =
            """
            using Modmynitro.SourceGenerators.Composition.Attributes;

            public interface ITestBase
            {
                int TestBase { get; }
            }
            
            public interface ITest : ITestBase
            {
                int Test { get; }
            }

            public class TestImplementation : ITest
            {
                public int Test { get; }
                
                public int TestBase { get; }
            }

            public partial class Composition
            {
                [CompositionAttribute(typeof(ITest))]
                private readonly TestImplementation _composition = new();
            }
            """;

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify<CompositionSourceGenerator>(source);
    }
    
    [Test]
    public Task Generic()
    {
        // The source code to test
        var source =
            """
            using Modmynitro.SourceGenerators.Composition.Attributes;

            public interface ITest<TInterface>
                where TInterface : class
            {
                TInterface Test { get; }
            }

            public class TestImplementation<TImpl> : ITest<TImpl>
                where TImpl : class
            {
                public TImpl Test { get; }
            }

            public partial class Composition<TComp>
                where TComp : class
            {
                [Composition(typeof(ITest<>))]
                private readonly TestImplementation<TComp> _composition = new();
            }
            """;

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify<CompositionSourceGenerator>(source);
    }
}