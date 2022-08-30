using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests;

public class ForAsGenericMethodTests : SubstituteDiagnosticVerifier
{
    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForInterface()
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
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForInterface_AndConstructorParametersUsed()
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
            var substitute = [|NSubstitute.Substitute.For<IFoo>(1)|];
            var otherSubstitute = [|NSubstitute.Substitute.For<IFoo>(constructorArguments: 1)|];
            var yetAnotherSubstitute = [|NSubstitute.Substitute.For<IFoo>(new [] { 1 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteConstructorArgumentsForInterfaceDescriptor, "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.For<IFoo>() instead.");
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForDelegate()
    {
        var source = @"using NSubstitute;
using System;
namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Func<int>>();
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForDelegate_AndConstructorParametersUsed()
    {
        var source = @"using NSubstitute;
using System;
namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Func<int>>(1)|];
            var otherSubstitute = [|NSubstitute.Substitute.For<Func<int>>(constructorArguments: 1)|];
            var yetAnotherSubstitute = [|NSubstitute.Substitute.For<Func<int>>(new [] { 1 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteConstructorArgumentsForDelegateDescriptor, "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.For<Func<int>>() instead.");
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleClasses()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo, Bar>()|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteMultipleClassesDescriptor);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleSameClasses()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo, Foo>();
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleInterfaces()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public interface IBar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo, IBar>();
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsInterfaceNotImplementedByClass()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo, Bar>();
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleGenericTypeParameters_ContainsClassWithoutMatchingConstructor()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<IFoo, Bar>(1)|];
            var otherSubstitute = [|NSubstitute.Substitute.For<IFoo, Bar>(constructorArguments: 1)|];
            var yetAnotherSubstitute = [|NSubstitute.Substitute.For<IFoo, Bar>(new [] { 1 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.IFoo, MyNamespace.Bar> do not match the number of constructor arguments for MyNamespace.Bar. Check the constructors for MyNamespace.Bar and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        private Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>()|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        internal Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>()|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        protected internal Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>()|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied()
    {
        var source = @"using NSubstitute;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
namespace MyNamespace
{
    public class Foo
    {
        internal Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied()
    {
        var source = @"using NSubstitute;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
namespace MyNamespace
{
    public class Foo
    {
        protected internal Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>(1, 2, 3)|];
            var otherSubstitute = [|NSubstitute.Substitute.For<Foo>(new [] { 1, 2, 3 })|];
            var yetAnotherSubstitute = [|NSubstitute.Substitute.For<Foo>(constructorArguments: new [] { 1, 2, 3 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>(1)|];
            var otherSubstitute = [|NSubstitute.Substitute.For<Foo>(constructorArguments: 1)|];
            var yetAnotherSubstitute = [|NSubstitute.Substitute.For<Foo>(new [] { 1 })|];
            var yetYetAnotherSubstitute = [|NSubstitute.Substitute.For<Foo>(constructorArguments: new [] { 1 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y = 1)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>(1)|];
            var otherSubstitute = [|NSubstitute.Substitute.For<Foo>(constructorArguments: 1)|];
            var yetAnotherSubstitute = [|NSubstitute.Substitute.For<Foo>(new [] { 1 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
    {
        var source = @"using NSubstitute;
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>()|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes)
    {
        var source = $@"using NSubstitute;
using System.Runtime.CompilerServices;
{assemblyAttributes}
namespace MyNamespace
{{
    internal class Foo
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes)
    {
        var source = $@"using NSubstitute;
using System.Runtime.CompilerServices;
{assemblyAttributes}
namespace MyNamespace
{{
    internal class Foo
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = [|NSubstitute.Substitute.For<Foo>()|];
        }}
    }}
}}";
        await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
    }

    [Theory]
    [InlineData("decimal x", "1")] // valid c# but doesnt work in NSubstitute
    [InlineData("int x", "1m")]
    [InlineData("int x", "1D")]
    [InlineData("List<int> x", "new List<int>().AsReadOnly()")]
    [InlineData("params int[] x", "1m")]
    [InlineData("params int[] x", "1, 1m")]
    [InlineData("int x", "new [] { 1 }")]
    [InlineData("int x", "new object()")]
    [InlineData("params int[] x", "new [] { 1m }")]
    [InlineData("params int[] x", "new [] { 1, 1m }")]
    [InlineData("params int[] x", "new object[] { 1m }")]
    [InlineData("params int[] x", "new object[] { 1, 1m }")]
    public override async Task ReportsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
    {
        var source = $@"using NSubstitute;
using System.Collections.Generic;
namespace MyNamespace
{{
    public class Foo
    {{
        public Foo({ctorValues})
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = [|NSubstitute.Substitute.For<Foo>({invocationValues})|];
        }}
    }}
}}";
        await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
    }

    [Theory]
    [InlineData("int x", "1")]
    [InlineData("float x", "'c'")]
    [InlineData("int x", "'c'")]
    [InlineData("IList<int> x", "new List<int>()")]
    [InlineData("IEnumerable<int> x", "new List<int>()")]
    [InlineData("IEnumerable<int> x", "new List<int>().AsReadOnly()")]
    [InlineData("IEnumerable<char> x", @"""value""")]
    [InlineData("int x, params string[] y", "1, \"foo\"")]
    [InlineData("int x", @"new object[] { 1 }")]
    [InlineData("int[] x", @"new int[] { 1 }")]
    [InlineData("object[] x , int y", @"new object[] { 1 }, 1")]
    [InlineData("int[] x , int y", @"new int[] { 1 }, 1")]
    [InlineData("", @"new object[] { }")]
    [InlineData("", "new object[] { 1, 2 }.ToArray()")] // actual values known at runtime only so constructor analysys skipped
    [InlineData("int x", "new object[] { null }")] // even though we pass null as first arg, this works fine with NSubstitute
    [InlineData("int x, int y", "new object[] { null, null }")] // even though we pass null as first arg, this works fine with NSubstitute
    [InlineData("int x, int y", "new object[] { 1, null }")] // even though we pass null as first arg, this works fine with NSubstitute
    [InlineData("int x, params string[] y", "new object[] { 1, \"foo\", \"foo\"  }")]
    public override async Task ReportsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
    {
        var source = $@"using NSubstitute;
using System.Collections.Generic;
using System.Linq;
namespace MyNamespace
{{
    public class Foo
    {{
        public Foo({ctorValues})
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>({invocationValues});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithGenericArgument()
    {
        var source = @"using NSubstitute;
namespace MyNamespace
{

    public class FooTests
    {
        public T Foo<T>() where T : class
        {
            return Substitute.For<T>();
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenParamsParametersNotProvided()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, params object[] y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>(1);
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenParamsParametersProvided()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, params object[] y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>(1, 2, 3);
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount_AndParamsParameterDefined()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y, params object[] z)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|NSubstitute.Substitute.For<Foo>(1)|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.For<MyNamespace.Foo> do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }
}