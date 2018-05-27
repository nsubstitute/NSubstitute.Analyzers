using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzersTests
{
    public class ForAsGenericMethodTests : SubstituteAnalyzerTests
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

            await VerifyCSharpDiagnostic(source);
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
                Message = "Can not provide constructor arguments when substituting for an interface.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
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
            await VerifyCSharpDiagnostic(source);
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
                Message = "Can not provide constructor arguments when substituting for a delegate.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
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

            await VerifyCSharpDiagnostic(source);
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

            await VerifyCSharpDiagnostic(source);
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

            await VerifyCSharpDiagnostic(source);
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
                Message = "Constructor parameters count mismatch.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

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
                Message = "Missing parameterless constructor.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

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
                Message = "Constructor parameters count mismatch.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

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
                Message = "Constructor parameters count mismatch.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

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
                Message = "Constructor parameters count mismatch.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

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
                Message = "Substitute for internal member.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        public override async Task
            ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
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
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
            await VerifyCSharpDiagnostic(source);
        }

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
                Message = "Substitute for internal member.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

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
            var substitute = NSubstitute.Substitute.For<Foo>(new object());
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

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        [InlineData("decimal x", "1")] // valid c# but doesnt work in NSubstitute
        [InlineData("int x", "1m")]
        [InlineData("int x", "1D")]
        [InlineData("int x", "1D")]
        [InlineData("List<int> x", "new List<int>().AsReadOnly()")]
        public override async Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(
            string ctorValues, string invocationValues)
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
                Message = "Unable to find matching constructor.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        [InlineData("int x", "1")]
        [InlineData("float x", "'c'")]
        [InlineData("int x", "'c'")]
        [InlineData("IList<int> x", "new List<int>()")]
        [InlineData("IEnumerable<int> x", "new List<int>()")]
        [InlineData("IEnumerable<int> x", "new List<int>().AsReadOnly()")]
        [InlineData("IEnumerable<char> x", @"""value""")]
        public override async Task ReturnsNoDiagnostic_WhenConstructorArgumentsAreImplicitlyConvertible(string ctorValues, string invocationValues)
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
            await VerifyCSharpDiagnostic(source);
        }
    }
}