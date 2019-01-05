using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public class ReturnsAsExtensionMethodTests : ReEntrantReturnsSetupDiagnosticVerifier
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
            substitute.Bar().Returns(ReturnThis(), OtherReturn());
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

            var firstArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            var secondArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

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
            substitute.Bar().Returns(ReturnThis(), OtherReturn());
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

            var firstArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            var secondArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

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
            substitute.Bar().Returns(ReturnThis(), OtherReturn());
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

            var firstArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            var secondArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

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
            substitute.Bar().Returns(ReturnThis(), OtherReturn());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            substitute.Foo().Returns(NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            sub.Foo().Returns(1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            var secondArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            var nestedArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

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
            substitute.Bar().Returns(x => ReturnThis());
        }

        private int ReturnThis()
        {
            return OtherReturn();
        }

        private int OtherReturn()
        {
            var substitute = Substitute.For<IBar>();
            substitute.Foo().Returns(NestedReturnThis());
            return 1;
        }

        private int NestedReturnThis()
        {
            return OtherNestedReturnThis();
        }

        private int OtherNestedReturnThis()
        {
            var sub = Substitute.For<IBar>();
            sub.Foo().Returns(1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            expectedDiagnostic.OverrideMessage(expectedMessage);

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
            substitute.Bar().Returns(bar);
        }}

        public IBar Bar()
        {{
            var substitute = Substitute.For<IBar>();
            substitute.Foo().Returns(1);
            return substitute;
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
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
            substitute.Bar().Returns({rootCall});
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
            await VerifyNoDiagnostic(source);
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
            substitute.Bar().Returns({rootCall});
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
            await VerifyNoDiagnostic(source);
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
            substitute.Bar().Returns({firstReturn}, {secondReturn});
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
            await VerifyNoDiagnostic(source);
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
            substitute.Bar().Returns(FooBar.ReturnThis());
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
            substitute.Foo().Returns(1);
            return 1;
        }
    }
}";

            var firstArgumentDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;
            firstArgumentDiagnostic.OverrideMessage("Returns() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: Returns(x => FooBar.ReturnThis()).");

            await VerifyDiagnostics(new[] { source, secondSource }, firstArgumentDiagnostic);
        }
    }
}