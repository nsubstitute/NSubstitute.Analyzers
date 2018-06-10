using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests
{
    public class ForAsNonGenericMethodTests : SubstituteDiagnosticVerifier
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, null);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForInterface_WhenEmptyArrayPassed()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, new object[] { });
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not provide constructor arguments when substituting for an interface.",
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Func<int>) }, null);
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Func<int>) }, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorArgumentsForDelegate,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not provide constructor arguments when substituting for a delegate.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("new [] { typeof(Bar), new Foo().GetType() }")]
        [InlineData("new [] { typeof(Bar), new Foo().GetType() }.ToArray()")]
        public async Task ReturnsNoDiagnostic_WhenProxyTypeCannotBeInfered(string proxyExpression)
        {
            var source = $@"using NSubstitute;
using System.Linq;
namespace MyNamespace
{{
    public class Foo
    {{
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var bar = new Bar();
            var substitute = NSubstitute.Substitute.For({proxyExpression}, null);
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleTypeParameters_ContainMultipleClasses()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo), typeof(Bar) }, null);
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
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleSameClasses()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo), typeof(Foo) }, null);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleInterfaces()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo), typeof(IBar) }, null);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsInterfaceNotImplementedByClass()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo), typeof(Bar) }, null);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleTypeParameters_ContainsClassWithoutMatchingConstructor()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo), typeof(Bar) }, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Constructor parameters count mismatch.",
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, null);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForWithoutAccessibleConstructor,
                Severity = DiagnosticSeverity.Warning,
                Message = "Missing parameterless constructor.",
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, new object[] { 1, 2, 3 });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Constructor parameters count mismatch.",
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Constructor parameters count mismatch.",
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Constructor parameters count mismatch.",
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, null);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForInternalMember,
                Severity = DiagnosticSeverity.Warning,
                Message = "Substitute for internal member.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            var source = @"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, null );
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
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
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, null);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForInternalMember,
                Severity = DiagnosticSeverity.Warning,
                Message = "Substitute for internal member.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenCorrespondingConstructorArgumentsNotCompatible()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(Foo) }, new [] { new object() } );
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Unable to find matching constructor.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("decimal x", "1")] // valid c# but doesnt work in NSubstitute
        [InlineData("int x", "1m")]
        [InlineData("int x", "1D")]
        [InlineData("List<int> x", "new List<int>().AsReadOnly()")]
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
            var substitute = NSubstitute.Substitute.For(new [] {{ typeof(Foo) }}, new object[] {{{invocationValues}}});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Unable to find matching constructor.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("int x", "new object [] { 1 }")]
        [InlineData("float x", "new object [] { 'c' }")]
        [InlineData("int x", "new object [] { 'c' }")]
        [InlineData("IList<int> x", "new object [] { new List<int>() }")]
        [InlineData("IEnumerable<int> x", "new object [] { new List<int>() }")]
        [InlineData("IEnumerable<int> x", "new object [] { new List<int>().AsReadOnly() }")]
        [InlineData("IEnumerable<char> x", @"new object [] { ""value"" }")]
        [InlineData("", @"new object[] { }")]
        [InlineData("", "new object[] { 1, 2 }.ToArray()")] // actual values known at runtime only so constructor analysys skipped
        public override async Task ReturnsNoDiagnostic_WhenConstructorArgumentsAreImplicitlyConvertible(string ctorValues, string invocationValues)
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
            var substitute = NSubstitute.Substitute.For(new [] {{ typeof(Foo) }}, {invocationValues});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }
    }
}