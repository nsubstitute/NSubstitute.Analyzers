using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class SyncOverAsyncThrowsCodeFixActionsTests : CSharpCodeFixActionsVerifier, ISyncOverAsyncThrowsCodeFixActionsVerifier
{
    protected override CodeFixProvider CodeFixProvider { get; } = new SyncOverAsyncThrowsCodeFixProvider();

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SyncOverAsyncThrowsAnalyzer();

    [Theory]
    [InlineData("Throws", "Replace with Returns")]
    [InlineData("ThrowsForAnyArgs", "Replace with ReturnsForAnyArgs")]
    public async Task CreatesCodeAction_WhenOverloadSupported(string method, string expectedCodeActionTitle)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar().{method}(new Exception());
        }}
    }}
}}";
        await VerifyCodeActions(source, NSubstituteVersion.NSubstitute4_2_2, expectedCodeActionTitle);
    }

    [Theory]
    [InlineData("Throws", "Replace with ThrowsAsync")]
    [InlineData("ThrowsForAnyArgs", "Replace with ThrowsAsyncForAnyArgs")]
    public async Task CreatesCodeAction_ForModernSyntax(string method, string expectedCodeActionTitle)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar().{method}(new Exception());
            substitute.Bar().{method}(callInfo => new Exception());
            substitute.Bar().{method}(createException: callInfo => new Exception());
        }}
    }}
}}";
        await VerifyCodeActions(source, Enumerable.Repeat(expectedCodeActionTitle, 3).ToArray());
    }

    [Theory]
    [InlineData("Throws")]
    [InlineData("ThrowsForAnyArgs")]
    public async Task DoesNotCreateCodeAction_WhenOverloadNotSupported(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar().{method}(callInfo => new Exception());
            ExceptionExtensions.{method}(substitute.Bar(), callInfo => new Exception());
        }}
    }}
}}";
        await VerifyCodeActions(source, NSubstituteVersion.NSubstitute4_2_2);
    }
}