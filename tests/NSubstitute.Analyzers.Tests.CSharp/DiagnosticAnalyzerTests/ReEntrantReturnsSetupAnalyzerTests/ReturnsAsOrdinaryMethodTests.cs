using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsAsOrdinaryMethodTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        [Theory]
        [InlineData("substitute.Foo().Returns(1);")]
        [InlineData("OtherReturn(); substitute.Foo().Returns(1);")]
        [InlineData("substitute.Foo().Returns<int>(1);")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        public async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            SubstituteExtensions.Returns(substitute.Bar(), ReturnThis(), OtherReturn());
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}
    }}
}}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 60)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 74)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("OtherReturn(); substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        public async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            SubstituteExtensions.Returns(substitute.Bar(), ReturnThis(), OtherReturn());
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}
    }}
}}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 60)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 74)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Fact]
        public async Task ReturnsDiagnostic_ForNestedReEntrantCall()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
    }

    public interface IBar
    {
        int Foo();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For<IFoo>();
            SubstituteExtensions.Returns(substitute.Bar(), ReturnThis(), OtherReturn());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            SubstituteExtensions.Returns(substitute.Foo(), NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            SubstituteExtensions.Returns(sub.Foo(), 1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 60)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 74)
                }
            };

            var nestedArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(31, 60)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic, nestedArgumentDiagnostic);
        }

        [Theory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("x => 1", "x => 2")]
        public async Task ReturnsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn)
        {
            var source = $@"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            SubstituteExtensions.Returns(substitute.Bar(), {firstReturn}, {secondReturn});
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
           return 1;
        }}

        private int ReturnThis(CallInfo info)
        {{
            return OtherReturn(info);
        }}

        private int OtherReturn(CallInfo info)
        {{
           return 1;
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }
    }
}