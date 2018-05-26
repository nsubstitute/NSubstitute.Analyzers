using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.SubstituteAnalyzersTests
{
    public class ForPartsOfMethodTests : SubstituteAnalyzerTests
    {
        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForInterface()
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
            var substitute = NSubstitute.Substitute.ForPartsOf<IFoo>();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can only substitute for parts of classes, not interfaces or delegates.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForDelegate()
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Func<int>>();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can only substitute for parts of classes, not interfaces or delegates.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 30)
                }
            };

            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>();
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(1, 2, 3);
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(1);
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

        // even though parameters are optional, NSubstitute requires to pass all of them
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(1);
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>();
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>();
        }
    }
}";
            await VerifyCSharpDiagnostic(source);
        }

        [Fact]
        public override async Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>();
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(new object());
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

        [Fact]
        public override async Task ReturnsDiagnostic_WhenAssigningDoubleToInt()
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
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(1m);
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

        // even though it is valid in c# NSubstitute will throw exception
        [Fact]
        public override async Task ReturnsDiagnostic_WhenAssigningIntToDouble()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(double x)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(1);
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

        [Fact]
        public override async Task ReturnsNoDiagnostic_WhenCorrespondingConstructorArgumentsAreImplicitlyConvertible()
        {
            var source = @"using NSubstitute;
using System.Collections.Generic;
namespace MyNamespace
{
    public class Foo
    {
        public Foo(IEnumerable<char> x)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.ForPartsOf<Foo>(""asfsafasdf"");
        }
    }
}";
            await VerifyCSharpDiagnostic(source);
        }
    }
}