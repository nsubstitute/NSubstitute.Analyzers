using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests
{
    public class ForAsGenericMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForInterface()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForInterface_AndConstructorParametersUsed()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.For<IFoo>() instead.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForDelegate()
        {
            var source = @"using NSubstitute;
using System;
namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Func<int>>();
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForDelegate_AndConstructorParametersUsed()
        {
            var source = @"using NSubstitute;
using System;
namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Func<int>>(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorArgumentsForDelegate,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.For<Func<int>>() instead.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleClasses()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo, Bar>();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteMultipleClasses,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleSameClasses()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo, Foo>();
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleInterfaces()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public interface IBar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo, IBar>();
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsInterfaceNotImplementedByClass()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo, Bar>();
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleGenericTypeParameters_ContainsClassWithoutMatchingConstructor()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo, Bar>(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.IFoo, MyNamespace.Bar> do not match the number of constructor arguments for MyNamespace.Bar. Check the constructors for MyNamespace.Bar and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        private Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForWithoutAccessibleConstructor,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>(1, 2, 3);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y = 1)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            var source = @"using NSubstitute;
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForInternalMember,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Can not substitute for internal type. To substitute for internal type expose your type to DynamicProxyGenAssembly2 via [assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task
            ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            var source = @"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task
            ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
        {
            var source = @"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""SomeValue"")]
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForInternalMember,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Can not substitute for internal type. To substitute for internal type expose your type to DynamicProxyGenAssembly2 via [assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("decimal x", "1")] // valid c# but doesnt work in NSubstitute
        [InlineData("int x", "1m")]
        [InlineData("int x", "1D")]
        [InlineData("List<int> x", "new List<int>().AsReadOnly()")]
        [InlineData("int x", "new [] { 1 }")]
        [InlineData("int x", "new object()")]
        public override async Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"using NSubstitute;
using System.Collections.Generic;
namespace MyNamespace
{{
    public class Foo
    {{
        public Foo({ctorValues})
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>({invocationValues});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("int x", "1")]
        [InlineData("float x", "'c'")]
        [InlineData("int x", "'c'")]
        [InlineData("IList<int> x", "new List<int>()")]
        [InlineData("IEnumerable<int> x", "new List<int>()")]
        [InlineData("IEnumerable<int> x", "new List<int>().AsReadOnly()")]
        [InlineData("IEnumerable<char> x", @"""value""")]
        [InlineData("int x", @"new object[] { 1 }")]
        [InlineData("int[] x", @"new int[] { 1 }")]
        [InlineData("object[] x , int y", @"new object[] { 1 }, 1")]
        [InlineData("int[] x , int y", @"new int[] { 1 }, 1")]
        [InlineData("", @"new object[] { }")]
        [InlineData("", "new object[] { 1, 2 }.ToArray()")] // actual values known at runtime only so constructor analysys skipped
        [InlineData("int x", "new object[] { null }")] // even though we pass null as first arg, this works fine with NSubstitute
        [InlineData("int x, int y", "new object[] { null, null }")] // even though we pass null as first arg, this works fine with NSubstitute
        [InlineData("int x, int y", "new object[] { 1, null }")] // even though we pass null as first arg, this works fine with NSubstitute
        public override async Task ReturnsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"using NSubstitute;
using System.Collections.Generic;
using System.Linq;
namespace MyNamespace
{{
    public class Foo
    {{
        public Foo({ctorValues})
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>({invocationValues});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReturnsNoDiagnostic_WhenUsedWithGenericArgument()
        {
            var source = @"using NSubstitute;
namespace MyNamespace
{

    public class FooTests
    {
        public T Foo<T>() where T : class
        {
            return Substitute.For<T>();
        }
    }
}";
            await VerifyDiagnostic(source);
        }
    }
}