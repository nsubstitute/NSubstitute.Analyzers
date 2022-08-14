using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberArgumentMatcherAnalyzerTests;

public class NonSubstitutableMemberArgumentMatcherTests : NonSubstitutableMemberArgumentMatcherDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedInNonVirtualMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int? firstArg)
        {{
            return 2;
        }}

        public int Bar(Action firstArg)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";
        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsDiagnostics_WhenUsedInStaticMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public static int Bar(int? firstArg)
        {{
            return 2;
        }}

        public static int Bar(Action firstArg)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo.Bar({arg});
        }}
    }}
}}";

        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInVirtualMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int? firstArg)
        {{
            return 2;
        }}

        public virtual int Bar(Action firstArg)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInNonSealedOverrideMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int? firstArg)
        {{
            return 2;
        }}

        public virtual int Bar(Action firstArg)
        {{
            return 2;
        }}
    }}

    public class Foo2 : Foo
    {{
        public override int Bar(int? firstArg) => 1;

        public override int Bar(Action firstArg) => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInDelegate(string arg)
    {
        var funcArgType = arg.EndsWith("Invoke()") ? "Action" : "int?";
        var source = $@"using NSubstitute;
using System;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<{funcArgType}, int>>();
            substitute({arg});
            substitute.When(x => x({arg}));
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedInSealedOverrideMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int? firstArg)
        {{
            return 2;
        }}

        public virtual int Bar(Action firstArg)
        {{
            return 2;
        }}
    }}

    public class Foo2 : Foo
    {{
        public sealed override int Bar(int? firstArg) => 1;

        public sealed override int Bar(Action firstArg) => 2;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";

        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInAbstractMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int? firstArg);

        public abstract int Bar(Action firstArg);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInInterfaceMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int? firstArg);

        int Bar(Action firstArg);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInGenericInterfaceMethod(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
   public interface IFoo<T>
    {{
        int Bar<T>(int? firstArg);

        int Bar<T>(Action firstArg);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            substitute.Bar<int>({arg});
            substitute.When(x => x.Bar<int>({arg}));
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInInterfaceIndexer(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int this[int? i] {{ get; }}

        int this[Action i] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            var _ = substitute[{arg}];
            substitute.When(x => _ = x[{arg}]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInVirtualIndexer(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int this[int? x] => 0;

        public virtual int this[Action x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var _ = substitute[{arg}];
            substitute.When(x => _ = x[{arg}]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedInNonVirtualIndexer(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int this[int? x] => 0;

        public int this[Action x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var _ = substitute[{arg}];
            substitute.When(x => _ = x[{arg}]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string arg)
    {
        var source = $@"using System;
using System.Linq.Expressions;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int? firstArg)
        {{
            return 1;
        }}

        public int Bar(Action firstArg)
        {{
            return 1;
        }}
    }}

    public class Arg
    {{
        public static T Any<T>()
        {{
            return default(T);
        }}
      
        public static T Is<T>(T value)
        {{
            return default(T);
        }}

        public static T Is<T>(Expression<Predicate<T>> predicate)
        {{
            return default(T);
        }}

        public static Action Invoke()
        {{
            return default(Action);
        }}

        public static T Do<T>(Action<T> value)
        {{
            return default(T);
        }}
        
        public static T InvokeDelegate<T>()
        {{
            return default(T);
        }}

        public static class Compat
        {{
            public static T Any<T>()
            {{
                return default(T);
            }}

            public static T Is<T>(T value)
            {{
                return default(T);
            }}

            public static T Is<T>(Expression<Predicate<T>> predicate)
            {{
                return default(T);
            }}

            public static Action Invoke()
            {{
                return default(Action);
            }}

            public static T Do<T>(Action<T> value)
            {{
                return default(T);
            }}

            public static T InvokeDelegate<T>()
            {{
                return default(T);
            }}
        }}  
    }}
    
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg});
            substitute.When(x => x.Bar({arg}));
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithPotentiallyValidAssignment(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var arg = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedAsStandaloneExpression(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            _ = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsDiagnostics_WhenUsedInConstructor(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public FooTests(int firstArg)
        {{
        }}

        public FooTests(int? firstArg)
        {{
        }}

        public FooTests(Action firstArg)
        {{
        }}

        public void Test()
        {{
            var x = new FooTests({arg});
            new FooTests({arg});
        }}
    }}
}}";
        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToNotApplied(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int FooBar(int? firstArg)
        {{
            return 1;
        }}

        internal virtual int FooBar(Action firstArg)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.FooBar({arg});
            substitute.When(x => x.FooBar({arg}));
        }}
    }}
}}";

        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToApplied(string arg)
    {
        var source = $@"using System;
using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int FooBar(int? firstArg)
        {{
            return 1;
        }}

        internal virtual int FooBar(Action firstArg)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.FooBar({arg});
            substitute.When(x => x.FooBar({arg}));
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string arg)
    {
        var source = $@"using System;
using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int FooBar(int? firstArg)
        {{
            return 1;
        }}

        internal virtual int FooBar(Action firstArg)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.FooBar({arg});
            substitute.When(x => x.FooBar({arg}));
        }}
    }}
}}";

        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedInProtectedInternalVirtualMember(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual int FooBar(int? firstArg)
        {{
            return 1;
        }}

        protected internal virtual int FooBar(Action firstArg)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.FooBar({arg});
            substitute.When(x => x.FooBar({arg}));
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string arg)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", ArgumentMatcherUsedWithoutSpecifyingCall.Id);
        Settings.Suppressions.Add(new Suppression
        {
            Target = "M:MyNamespace.Foo.Bar(System.Action,System.Action)",
            Rules = new List<string> { ArgumentMatcherUsedWithoutSpecifyingCall.Id }
        });
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 1;
        }}

        public int Bar(int x, int y)
        {{
            return 2;
        }}

        public int Bar(Action x)
        {{
            return 1;
        }}

        public int Bar(Action x, Action y)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg}, {arg});
            substitute.Bar([|{arg}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsNoDiagnostics_WhenSubscribingToEvent()
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

    public override async Task ReportsNoDiagnostics_WhenAssigningAllowedArgMatchersToSubstitutableMember(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Foo {{ get; set; }}
        int this[int x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Foo = {arg};
            substitute[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When", "WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int? Bar {{ get; set; }}

        public int? this[int? x] {{ get {{ return Bar; }} set {{ Bar = value; }} }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute.{whenMethod}(x => _ = x.Bar = {arg});
            substitute.{whenMethod}(x => x[1] = {arg});
        }}
    }}
}}";
        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    [CombinatoryData("When", "WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenAssigningArgMatchersToSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int? Bar {{ get; set; }}

        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute.{whenMethod}(x => _ = x.Bar = {arg});
            substitute.{whenMethod}(x => x[1] = {arg});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int? Bar {{ get; set; }}

        public int? this[int? x] {{ get {{ return Bar; }} set {{ Bar = value; }} }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute.Bar = {arg};
            substitute[1] = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    public override async Task ReportsDiagnostics_WhenDirectlyAssigningNotAllowedArgMatchersToMember(string arg)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Foo {{ get; set; }}
        int this[int x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Foo = {arg};
            substitute[1] = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received()",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceive()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethod(string receivedMethod, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{receivedMethod}.Foo = {arg};
            substitute.{receivedMethod}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}