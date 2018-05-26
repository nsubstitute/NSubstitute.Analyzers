using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.SubstituteAnalyzersTests
{
    public class ForMethodTests : SubstituteAnalyzerTests
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

namespace MyNamespace
{
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
                Message = "Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.",
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
        public async Task ReturnsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsClassWithoutMatchingConstructor()
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

        public override Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenCorrespondingConstructorArgumentsNotCompatible()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenAssigningDoubleToInt()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsDiagnostic_WhenAssigningIntToDouble()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReturnsNoDiagnostic_WhenCorrespondingConstructorArgumentsAreImplicitlyConvertible()
        {
            throw new System.NotImplementedException();
        }
    }
}