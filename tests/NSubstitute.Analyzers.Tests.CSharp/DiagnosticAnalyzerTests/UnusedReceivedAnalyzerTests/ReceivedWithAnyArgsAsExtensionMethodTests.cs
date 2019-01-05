using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public class ReceivedWithAnyArgsAsExtensionMethodTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall()
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
            substitute.ReceivedWithAnyArgs();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class FooBar
    {
    }

    public interface IFoo
    {
        FooBar Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.ReceivedWithAnyArgs().Bar();
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{

    public interface IFoo
    {
        int Bar { get; set; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            var bar = substitute.ReceivedWithAnyArgs().Bar;
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{

    public interface IFoo
    {
        int this[int x] { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            var bar = substitute.ReceivedWithAnyArgs()[0];
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate()
        {
            var source = @"using System;
using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Func<int>>();
            substitute.ReceivedWithAnyArgs()();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod()
        {
            var source = @"using System;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static T ReceivedWithAnyArgs<T>(this T substitute, decimal x) where T : class
        {
            return null;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            object substitute = null;
            substitute.ReceivedWithAnyArgs(1m);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }
    }
}