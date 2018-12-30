using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsForAnyArgsAsExtensionMethodWithGenericTypeSpecifiedTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall)
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
            substitute.Bar().ReturnsForAnyArgs<int>(ReturnThis(), OtherReturn());
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
                    "ReturnsForAnyArgs() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 53)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 67)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall)
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
            substitute.Bar().ReturnsForAnyArgs<int>(ReturnThis(), OtherReturn());
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
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 53)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 67)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall)
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
            substitute.Bar().ReturnsForAnyArgs<int>(ReturnThis(), OtherReturn());
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
                    "ReturnsForAnyArgs() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 53)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 67)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_ForNestedReEntrantCall()
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
            substitute.Bar().ReturnsForAnyArgs<int>(ReturnThis(), OtherReturn());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            substitute.Foo().ReturnsForAnyArgs<int>(NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            sub.Foo().ReturnsForAnyArgs<int>(1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 53)
                }
            };

            var secondArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => OtherReturn()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 67)
                }
            };

            var nestedArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(31, 53)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic, secondArgumentDiagnostic, nestedArgumentDiagnostic);
        }

        public override async Task ReportsDiagnostic_ForSpecificNestedReEntrantCall()
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
            substitute.Bar().ReturnsForAnyArgs<int>(x => ReturnThis());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            substitute.Foo().ReturnsForAnyArgs<int>(NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            sub.Foo().ReturnsForAnyArgs<int>(1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => NestedReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(31, 53)
                }
            };

            await VerifyDiagnostic(source, firstArgumentDiagnostic);
        }

        public override async Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string localVariable)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        IBar Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {localVariable}
            var substitute = Substitute.For<IFoo>();
            substitute.Bar().ReturnsForAnyArgs<IBar>(bar);
        }}

        public IBar Bar()
        {{
            var substitute = Substitute.For<IBar>();
            substitute.Foo().Returns(1);
            return substitute;
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall)
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
            substitute.Bar().ReturnsForAnyArgs<int>({rootCall});
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

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall)
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
            substitute.Bar().ReturnsForAnyArgs<int>({rootCall});
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

        public override async Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn)
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
            substitute.Bar().ReturnsForAnyArgs<int>({firstReturn}, {secondReturn});
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

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles()
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
            substitute.Bar().ReturnsForAnyArgs<int>(FooBar.ReturnThis());
        }
    }
}";

            var secondSource = @"
using NSubstitute;

namespace MyNamespace
{
    public class FooBar
    {
        public static int ReturnThis()
        {
            var substitute = Substitute.For<IBar>();
            substitute.Foo().ReturnsForAnyArgs<int>(1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ReEntrantSubstituteCall,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "ReturnsForAnyArgs() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: ReturnsForAnyArgs(x => FooBar.ReturnThis()).",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 53)
                }
            };

            await VerifyDiagnostic(new[] { source, secondSource }, firstArgumentDiagnostic);
        }
    }
}