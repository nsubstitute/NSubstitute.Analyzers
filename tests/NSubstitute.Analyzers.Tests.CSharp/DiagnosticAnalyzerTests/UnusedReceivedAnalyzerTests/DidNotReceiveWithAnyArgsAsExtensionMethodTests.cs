﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public class DidNotReceiveWithAnyArgsAsExtensionMethodTests : UnusedReceivedDiagnosticVerifier
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
            substitute.DidNotReceiveWithAnyArgs();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.UnusedReceived,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Unused received check. To fix, make sure there is a call after ""DidNotReceiveWithAnyArgs"". Correct: ""sub.DidNotReceiveWithAnyArgs().SomeCall();"". Incorrect: ""sub.DidNotReceiveWithAnyArgs();""",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 13)
                }
            };

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
            substitute.DidNotReceiveWithAnyArgs().Bar();
        }
    }
}";

            await VerifyDiagnostic(source);
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
            var bar = substitute.DidNotReceiveWithAnyArgs().Bar;
        }
    }
}";

            await VerifyDiagnostic(source);
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
            var bar = substitute.DidNotReceiveWithAnyArgs()[0];
        }
    }
}";

            await VerifyDiagnostic(source);
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
            substitute.DidNotReceiveWithAnyArgs()();
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod()
        {
            var source = @"using System;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, params int[] @params) where T : class
        {
            return null;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            object substitute = null;
            substitute.DidNotReceiveWithAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostic(source);
        }
    }
}