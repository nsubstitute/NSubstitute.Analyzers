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
            substitute.When(x => {{ new Bar().FooBar({arg}, {arg});}});
        }}
    }}
}}";
            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
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
            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
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
    }
}