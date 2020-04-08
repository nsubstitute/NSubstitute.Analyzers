using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.AsyncReceivedInOrderCallbackAnalyzerTests
{
    public class AsyncReceivedInOrderCallbackDiagnosticVerifier : CSharpDiagnosticVerifier, IAsyncReceivedInOrderCallbackDiagnosticVerifier
    {
        private readonly DiagnosticDescriptor _descriptor = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.AsyncCallbackUsedInReceivedInOrder;

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new AsyncReceivedInOrderCallbackAnalyzer();

        [Fact]
        public async Task ReportsDiagnostic_WhenAsyncLambdaCallbackUsedInReceivedInOrder()
        {
           var source = @"using System.Threading.Tasks;
using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        Task<int> Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder([|async|] () =>
            { 
                _ = await substitute.Bar; 
            });
        }
    }
}";

           await VerifyDiagnostic(source, _descriptor);
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenAsyncDelegateCallbackUsedInReceivedInOrder()
        {
           var source = @"using System.Threading.Tasks;
using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        Task<int> Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder([|async|] delegate
            { 
                _ = await substitute.Bar; 
            });
        }
    }
}";

           await VerifyDiagnostic(source, _descriptor);
        }

        [Fact]
        public async Task ReportsNoDiagnostic_WhenNonAsyncLambdaCallbackUsedInReceivedInOrder()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            { 
                _ = substitute.Bar; 
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostic_WhenNonAsyncDelegateCallbackUsedInReceivedInOrder()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(delegate 
            { 
                _ = substitute.Bar; 
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod()
        {
            var source = @"using System;
using System.Threading.Tasks;

namespace MyNamespace
{
    public interface IFoo
    {
        Task<int> Bar { get; }
    }

    public class Received
    {
        public static void InOrder(Action action)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            IFoo substitute = null; 
            Received.InOrder(async () =>
            { 
                _ = await substitute.Bar; 
            });
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }
    }
}