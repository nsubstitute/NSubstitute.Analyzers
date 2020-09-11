﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.NonSubstitutableMemberReceivedInOrderSuppressDiagnosticsCodeFixProviderTests
{
    public class NonSubstitutableMemberReceivedInOrderSuppressDiagnosticsCodeFixVerifier : CSharpSuppressDiagnosticSettingsVerifier, INonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider { get; } = new NonSubstitutableMemberSuppressDiagnosticsCodeFixProvider();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedInOrderAnalyzer();

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                substitute.Bar();
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public static int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            Received.InOrder(() =>
            {
                Foo.Bar();
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForExtensionMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            Received.InOrder(() =>
            {
                substitute.GetBar();
            });
        }
    }

    public static class MyExtensions
    {
        public static IBar Bar { get; set; }

        public static int GetBar(this object @object)
        {
            return Bar.Foo(@object);
        }
    }

    public interface IBar
    {
        int Foo(object @obj);
    }
}";

            await VerifySuppressionSettings(source, "M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class Foo2 : Foo
    {
        public sealed override int Bar() => 1;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
            Received.InOrder(() =>
            {
                substitute.Bar();
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "M:MyNamespace.Foo2.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                _ = substitute.Bar;
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                _ = substitute[1];
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                _ = substitute[1];
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification, 1);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                _ = substitute[1];
            });
        }
    }
}";

            await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification, 2);
        }
    }
}