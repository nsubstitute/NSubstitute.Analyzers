using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public class ArgumentMatcherMisuseTests : ArgumentMatcherMisuseDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForMethodCall(string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x, int y);
    }}

    public class Bar
    {{
        public int I {{ get; set; }}
        public Bar(){{}}

        public Bar(int i){{}}

        public int FooBar(int x, int y)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg}, {arg});
            var bar = substitute.Bar({arg}, {arg});
            new Bar().FooBar({arg}, {arg});
            new Bar({arg});
            new Bar {{ I = {arg}}};
            var anonymous = new {{ I = {arg}}};
            substitute.When(x => {{ new Bar().FooBar({arg}, {arg});}});
        }}
    }}
}}";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForIndexerCall(string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int this[int x, int y] {{ get; }}
    }}

    public class Bar
    {{
        public int this[int x, int y] => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            _ = substitute[{arg}, {arg}];
            _ = new Bar()[{arg}, {arg}];
            substitute.When(x => {{ _ = new Bar()[{arg}, {arg}];}});
        }}
    }}
}}";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedArgMethod(string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x, int y);
    }}

    public class Bar
    {{
        public int FooBar(int x, int y)
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

        public static T Invoke<T>(T value)
        {{
            return default(T);
        }}

        public static T Do<T>(T value)
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

            public static T Invoke<T>(T value)
            {{
                return default(T);
            }}

            public static T Do<T>(T value)
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
            substitute.Bar({arg}, {arg});
            var bar = substitute.Bar({arg}, {arg});
            new Bar().FooBar({arg}, {arg});
            substitute.When(x => {{ new Bar().FooBar({arg}, {arg});}});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUseTogetherWithUnfortunatelyNamedArgDoInvoke(string argDoInvoke, string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x, int y);
    }}

    public class Bar
    {{
        public int FooBar(int x, int y)
        {{
            return 1;
        }}
    }}

    public class Arg
    {{
        public static T Invoke<T>(T value)
        {{
            return default(T);
        }}

        public static T Do<T>(T value)
        {{
            return default(T);
        }}

        public static T InvokeDelegate<T>(T value)
        {{
            return default(T);
        }}
        
        public static class Compat
        {{
            public static T Invoke<T>(T value)
            {{
                return default(T);
            }}

            public static T InvokeDelegate<T>(T value)
            {{
                return default(T);
            }}
            
            public static T Do<T>(T value)
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
            substitute.Bar({arg}, {argDoInvoke});
            var bar = substitute.Bar({argDoInvoke}, {arg});
            new Bar().FooBar({arg}, {argDoInvoke});
            substitute.When(x => {{ new Bar().FooBar({argDoInvoke}, {arg});}});
        }}
    }}
}}";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForMethodCall(string argDo, string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x, int y);
    }}

    public class Bar
    {{
        public int FooBar(int x, int y)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg}, {argDo});
            var bar = substitute.Bar({argDo}, {arg});
            new Bar().FooBar({arg}, {argDo});
            substitute.When(x => {{ new Bar().FooBar({argDo}, {arg});}});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForIndexerCall(string argDo, string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int this[int x, int y] {{ get; }}
    }}

    public class Bar
    {{
        public int this[int x, int y] => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            _ = substitute[{arg}, {argDo}];
            _ = new Bar()[{argDo}, {arg}];
            substitute.When(x => {{ _ = new Bar()[{argDo}, {arg}];}});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForMethodCall(string argInvoke, string arg)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x, Action<int> y);
    }}

    public class Bar
    {{
        public int FooBar(int x, Action<int> y)
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar({arg}, {argInvoke});
            var bar = substitute.Bar({arg}, {argInvoke});
            new Bar().FooBar({arg}, {argInvoke});
            substitute.When(x => {{ new Bar().FooBar({arg}, {argInvoke});}});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForIndexerCall(string argInvoke, string arg)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int this[int x, Action<int> y] {{ get; }}
    }}

    public class Bar
    {{
        public int this[int x, Action<int> y] => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            _ = substitute[{arg}, {argInvoke}];
            _ = new Bar()[{arg}, {argInvoke}];
            substitute.When(x => {{ _ = new Bar()[{arg}, {argInvoke}];}});
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
    }
}