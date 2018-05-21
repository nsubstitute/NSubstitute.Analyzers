using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.UnusedReceivedAnalyzerTests
{
    public class DidNotReceiveAsExtensionMethodTests : UnusedReceivedAnalyzerTests
    {
        [Fact]
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
            substitute.DidNotReceive();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.UnusedReceived,
                Severity = DiagnosticSeverity.Warning,
                Message = "Unused received check.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 13)
                }
            };


            await VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }


        [Fact]
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
            substitute.DidNotReceive().Bar();
        }
    }
}";

            await VerifyCSharpDiagnostic(source);
        }

        [Fact]
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
            var bar = substitute.DidNotReceive().Bar;
        }
    }
}";

            await VerifyCSharpDiagnostic(source);
        }

        [Fact]
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
            var bar = substitute.DidNotReceive()[0];
        }
    }
}";

            await VerifyCSharpDiagnostic(source);
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
            substitute.DidNotReceive()();
        }
    }
}";
            await VerifyCSharpDiagnostic(source);
        }
    }
}