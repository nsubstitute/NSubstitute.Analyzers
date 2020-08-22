using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedInOrderAnalyzerTests
{
    public class NonSubstitutableMemberReceivedInOrderDiagnosticVerifier : CSharpDiagnosticVerifier, INonSubstitutableMemberReceivedInOrderDiagnosticVerifier
    {
        internal AnalyzersSettings Settings { get; set; }

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedInOrderAnalyzer();

        private readonly DiagnosticDescriptor _internalSetupSpecificationDescriptor = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

        private readonly DiagnosticDescriptor _nonVirtualReceivedInOrderSetupSpecificationDescriptor = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualReceivedInOrderSetupSpecification;

        protected override string AnalyzerSettings => Settings != null ? Json.Encode(Settings) : null;

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingNonVirtualMethodWithoutAssignment()
        {
            var source = @"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{
    public class Foo
    {

        public int Bar()
        {
            return 2;
        }
    }

    public class FooBar
    {
        public Task Bar()
        {
           return Task.CompletedTask; 
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var otherSubstitute = NSubstitute.Substitute.For<FooBar>();
            Received.InOrder(() => [|substitute.Bar()|]);
            Received.InOrder(async () => await [|otherSubstitute.Bar()|]);
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingNonVirtualMethodWithNonUsedAssignment()
        {
            var source = @"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }

        public Foo Bar(int x)
        {
            return null;
        }
    }

    public class FooBar
    {
        public Task<int> Bar()
        {
           return Task.FromResult(1);
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var otherSubstitute = NSubstitute.Substitute.For<FooBar>();
            Received.InOrder(() =>
            {
                [|substitute.Bar()|]; 
                var x = (int)[|substitute.Bar()|]; 
                var y = [|substitute.Bar()|]; 
                var z = (int)[|substitute.Bar()|]; 
                var zz = [|substitute.Bar()|] as object; 
            });
            Received.InOrder(async () =>
            {
                var x = await [|otherSubstitute.Bar()|];
            });
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingNonVirtualPropertyWithoutAssignment()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; set;}
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                _ = [|substitute.Bar|]; 
            });
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingNonVirtualPropertyWithNonUsedAssignment()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; set;}
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                var x = [|substitute.Bar|]; 
            });
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingNonVirtualIndexerWithoutAssignment()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] { get => 1; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                _ = [|substitute[1]|]; 
            });
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingNonVirtualIndexerWithNonUsedAssignment()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] { get => 1; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                var x = [|substitute[1]|]; 
            });
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenSubscribingToEvent()
        {
            var source = @"using NSubstitute;
using System;
namespace MyNamespace
{
    public class Foo
    {
        public event Action SomeEvent;
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
            Received.InOrder(() => substitute.SomeEvent += Arg.Any<Action>());
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenNonVirtualMethodUsedAsPartOfExpression_WithoutAssignment()
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
                var a = [|substitute.Bar()|] + [|substitute.Bar()|];
                var b = [|substitute.Bar()|] - [|substitute.Bar()|];
            });
        }
    }
}";
            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenNonVirtualMethodUsedAsPartOfExpression_WithAssignment()
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
                var a = substitute.Bar() + substitute.Bar();
                var b = substitute.Bar() - substitute.Bar();
                var aa = a;
                var bb = b;
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenInvokingNonVirtualMethodWithUsedAssignment()
        {
            var source = @"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooBar
    {
        public Task<int> Bar()
        {
           return Task.FromResult(1);
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var otherSubstitute = NSubstitute.Substitute.For<FooBar>();
            Received.InOrder(() =>
            {
                var a = substitute.Bar(); 
                var b = (int)substitute.Bar(); 
                var c = substitute.Bar() as object; 
                var aa = a;
                var bb = b;
                var cc = c;
            });
            Received.InOrder(async () =>
            {
                var a = await otherSubstitute.Bar();
                var aa = a;
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenInvokingNonVirtualPropertyWithUsedAssignment()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; set; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                var x = substitute.Bar;
                var y = x;
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenInvokingNonVirtualIndexerWithUsedAssignment()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] { get => 1; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                var x = substitute[1];
                var y = x;
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenNonVirtualMethodIsCalledAsArgument()
        {
            var source = @"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar(object x)
        {
            return 2;
        }

        public int FooBar()
        {
            return 1;
        }    
    }

    public class FooBar
    {
        public Task<int> Bar()
        {
           return Task.FromResult(1);
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var otherSubstitute = NSubstitute.Substitute.For<FooBar>();
            Received.InOrder(() =>
            {
                var local = 1;
                var x = substitute.Bar(substitute.FooBar()); 
                substitute.Bar(substitute.FooBar());
                substitute.Bar((int)substitute.FooBar());
                substitute.Bar(substitute.FooBar() as object);
                substitute.Bar(local);
                substitute.Bar(1);
            });
            Received.InOrder(async () =>
            {
                substitute.Bar(await otherSubstitute.Bar());
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenNonVirtualPropertyIsCalledAsArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar(int x)
        {
            return 2;
        }

        public int FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                var x = substitute.Bar(substitute.FooBar); 
                substitute.Bar(substitute.FooBar);
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenNonVirtualIndexerIsCalledAsArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar(int x)
        {
            return 2;
        }

        public int this[int x] { get { return 1; } }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                var x = substitute.Bar(substitute[1]); 
                substitute.Bar(substitute[1]);
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenInvokingProtectedInternalVirtualMember()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        protected internal virtual int Bar { get; }

        protected internal virtual int FooBar()
        {
            return 1;
        }

        protected internal virtual int this[int x]
        {
            get { return 1; }
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                substitute.FooBar();
                var x = substitute.Bar; 
                var y = substitute[1]; 
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenInvokingVirtualMember()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        protected internal virtual int Bar { get; }

        protected internal virtual int FooBar()
        {
            return 1;
        }

        protected internal virtual int this[int x]
        {
            get { return 1; }
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                substitute.FooBar();
                var x = substitute.Bar; 
                var y = substitute[1]; 
            });
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenInvokingInternalVirtualMember_AndInternalsVisibleToNotApplied()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        internal virtual int Bar { get; }

        internal virtual int FooBar()
        {
            return 1;
        }

        internal virtual int this[int x]
        {
            get { return 1; }
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                [|substitute.FooBar()|];
                var x = [|substitute.Bar|]; 
                var y = [|substitute[1]|]; 
            });
        }
    }
}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.",
                "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.",
                "Internal member this[] can not be intercepted without InternalsVisibleToAttribute."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(_internalSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        [Fact]
        public async Task ReportsNoDiagnostics_WhenInvokingInternalVirtualMember_AndInternalsVisibleToApplied()
        {
            var source = @"using System;
using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]

namespace MyNamespace
{
    public class Foo
    {
        internal virtual int Bar { get; }

        internal virtual int FooBar()
        {
            return 1;
        }

        internal virtual int this[int x]
        {
            get { return 1; }
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                substitute.FooBar();
                var x = substitute.Bar; 
                var y = substitute[1]; 
            });
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", _nonVirtualReceivedInOrderSetupSpecificationDescriptor.Id);

            var source = @"using System;
using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar(int x)
        {
            return 1;
        }

        public int Bar(int x, int y)
        {
            return 2;
        }

        public int Bar(Action x)
        {
            return 1;
        }

    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            Received.InOrder(() =>
            {
                substitute.Bar(1, 1);
                [|substitute.Bar(1)|];
            });
        }
    }
}";

            await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }
    }
}