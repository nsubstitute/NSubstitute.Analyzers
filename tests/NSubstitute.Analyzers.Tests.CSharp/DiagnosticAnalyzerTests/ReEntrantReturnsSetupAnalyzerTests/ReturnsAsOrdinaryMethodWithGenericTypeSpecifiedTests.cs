using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsAsOrdinaryMethodWithGenericTypeSpecifiedTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        [Theory]
        [InlineData("substitute.Foo().Returns(1);")]
        [InlineData("OtherReturn(); substitute.Foo().Returns(1);")]
        [InlineData("substitute.Foo().Returns<int>(1);")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        public override async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.Returns<int>(substitute.Bar(), ReturnThis(), OtherReturn());
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
                    new DiagnosticResultLocation(20, 65)
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
                    new DiagnosticResultLocation(20, 79)
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
        public override async Task ReturnsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall)
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
            SubstituteExtensions.Returns<int>(substitute.Bar(), ReturnThis(), OtherReturn());
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
                    new DiagnosticResultLocation(20, 65)
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
                    new DiagnosticResultLocation(20, 79)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Theory]
        [InlineData("substitute.When(x => x.Foo()).Do(callInfo => { });")]
        [InlineData("OtherReturn(); substitute.When(x => x.Foo()).Do(callInfo => { });")]
        public override async Task ReturnsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall)
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
            SubstituteExtensions.Returns<int>(substitute.Bar(), ReturnThis(), OtherReturn());
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
                    "Returns() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 65)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 79)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_ForNestedReEntrantCall()
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
            SubstituteExtensions.Returns<int>(substitute.Bar(), ReturnThis(), OtherReturn());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            SubstituteExtensions.Returns<int>(substitute.Foo(), NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            SubstituteExtensions.Returns<int>(sub.Foo(), 1);
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
                    new DiagnosticResultLocation(20, 65)
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
                    new DiagnosticResultLocation(20, 79)
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
                    new DiagnosticResultLocation(31, 65)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic, nestedArgumentDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_ForSpecificNestedReEntrantCall()
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
            SubstituteExtensions.Returns<int>(substitute.Bar(), x => ReturnThis());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            SubstituteExtensions.Returns<int>(substitute.Foo(), NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            SubstituteExtensions.Returns<int>(sub.Foo(), 1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(31, 65)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic);
        }

        [Theory]
        [InlineData("MyMethod()", "substitute.Foo().Returns(1);")]
        [InlineData("MyProperty", "substitute.Foo().Returns(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().Returns(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().Returns(1);")]
        [InlineData("MyMethod()", "substitute.Foo().Returns<int>(1);")]
        [InlineData("MyProperty", "substitute.Foo().Returns<int>(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().Returns<int>(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().Returns<int>(1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        public override async Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall)
        {
            var source = $@"using NSubstitute;
using NSubstitute.Core;
using System;

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
            SubstituteExtensions.Returns<int>(substitute.Bar(), {rootCall});
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

        private int ReturnThisWithCallInfo(CallInfo info)
        {{
            return OtherReturn();
        }}

        Func<CallInfo, int> MyMethod()
        {{
            return ReturnThisWithCallInfo;
        }}

        Func<CallInfo, int> MyProperty
        {{
            get {{ return ReturnThisWithCallInfo; }}
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        public override async Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall)
        {
            var source = $@"using NSubstitute;
using NSubstitute.Core;
using System;

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
            SubstituteExtensions.Returns<int>(substitute.Bar(), {rootCall});
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

        private int ReturnThisWithCallInfo(CallInfo info)
        {{
            return OtherReturn();
        }}

        Func<CallInfo, int> MyMethod()
        {{
            return ReturnThisWithCallInfo;
        }}

        Func<CallInfo, int> MyProperty
        {{
            get {{ return ReturnThisWithCallInfo; }}
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("x => 1", "x => 2")]
        public override async Task ReturnsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn)
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
            SubstituteExtensions.Returns<int>(substitute.Bar(), {firstReturn}, {secondReturn});
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