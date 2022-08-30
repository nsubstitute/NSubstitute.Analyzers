using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests;

public class SubstituteFactoryCreateMethodTests : SubstituteDiagnosticVerifier
{
    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForInterface()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(IFoo)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(IFoo)});
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForInterface_AndConstructorParametersUsed()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, new object[] { 1 })|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(IFoo)}, constructorArguments: new object[] { 1 })|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[] { 1 }, typesToProxy: new[] {typeof(IFoo)})|];
        }
    }
}";

        await VerifyDiagnostic(source, SubstituteConstructorArgumentsForInterfaceDescriptor, "Can not provide constructor arguments when substituting for an interface. Use SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)},null) instead.");
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForDelegate()
    {
        var source = @"using System;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Func<int>)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Func<int>)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Func<int>)});
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForDelegate_AndConstructorParametersUsed()
    {
        var source = @"using System;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Func<int>)}, new object[] { 1 })|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Func<int>)}, constructorArguments: new object[] { 1 })|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[] { 1 }, typesToProxy: new[] {typeof(Func<int>)})|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteConstructorArgumentsForDelegateDescriptor, "Can not provide constructor arguments when substituting for a delegate. Use SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Func<int>)},null) instead.");
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleTypeParameters_ContainsMultipleClasses()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo), typeof(Bar)}, null)|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new [] { typeof(Foo), typeof(Bar)}, constructorArguments: null)|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new [] { typeof(Foo), typeof(Bar)})|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteMultipleClassesDescriptor);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleSameClasses()
    {
        var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo), typeof(Foo)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new [] { typeof(Foo), typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new [] { typeof(Foo), typeof(Foo)});
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleInterfaces()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(IFoo), typeof(IBar)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new [] { typeof(IFoo), typeof(IBar)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new [] { typeof(IFoo), typeof(IBar)});

        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleTypeParameters_ContainsInterfaceNotImplementedByClass()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(IFoo), typeof(Bar) }, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new [] { typeof(IFoo), typeof(Bar) }, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new [] { typeof(IFoo), typeof(Bar) });
        }
    }
}";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleTypeParameters_ContainsClassWithoutMatchingConstructor()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(IFoo), typeof(Bar) }, new object[] { 1 })|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new [] { typeof(IFoo), typeof(Bar) }, constructorArguments: new object[] { 1 })|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[] { 1 }, typesToProxy: new [] { typeof(IFoo), typeof(Bar) })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.Create do not match the number of constructor arguments for MyNamespace.Bar. Check the constructors for MyNamespace.Bar and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null)|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null)|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Foo)})|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null)|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null)|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Foo)})|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null)|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null)|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Foo)})|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied()
    {
        var source = @"using NSubstitute.Core;
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied()
    {
        var source = @"using NSubstitute.Core;
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, new object[]{ 1, 2, 3})|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: new object[]{ 1, 2, 3})|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[]{ 1, 2, 3}, typesToProxy: new[] {typeof(Foo)})|];
        }
    }
}";

        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.Create do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
    {
        var source = @"using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, new object[]{ 1 })|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: new object[]{ 1 })|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[]{ 1 }, typesToProxy: new[] {typeof(Foo)})|];
        }
    }
}";

        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.Create do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, new object[]{ 1 })|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: new object[]{ 1 })|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[]{ 1 }, typesToProxy: new[] {typeof(Foo)} )|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.Create do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    [Fact]
    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
    {
        var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null)|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null)|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {typeof(Foo)})|];
        }
    }
}
";
        await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes)
    {
        var source = $@"using System.Runtime.CompilerServices;
using NSubstitute.Core;

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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {{typeof(Foo)}}, null);
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {{typeof(Foo)}}, constructorArguments: null);
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {{typeof(Foo)}});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes)
    {
        var source = $@"using System.Runtime.CompilerServices;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {{typeof(Foo)}}, null)|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {{typeof(Foo)}}, constructorArguments: null)|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new[] {{typeof(Foo)}})|];
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
    [InlineData("int x", "new object()")]
    [InlineData("params int[] x", "new [] { 1m }")]
    [InlineData("params int[] x", "new [] { 1, 1m }")]
    [InlineData("params int[] x", "new object[] { 1m }")]
    [InlineData("params int[] x", "new object[] { 1, 1m }")]
    public override async Task ReportsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
    {
        var source = $@"using System.Collections.Generic;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new[] {{typeof(Foo)}}, new object[] {{{invocationValues}}})|];
            var otherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {{typeof(Foo)}}, constructorArguments: new object[] {{{invocationValues}}})|];
            var yetAnotherSubstitute = [|SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: new object[] {{{invocationValues}}}, typesToProxy: new[] {{typeof(Foo)}})|];
        }}
    }}
}}";
        await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Core.ISubstituteFactory.Create do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
    }

    [Theory]
    [InlineData("int x", "new object [] { 1 }")]
    [InlineData("float x", "new object [] { 'c' }")]
    [InlineData("int x", "new object [] { 'c' }")]
    [InlineData("IList<int> x", "new object [] { new List<int>() }")]
    [InlineData("IEnumerable<int> x", "new object [] { new List<int>() }")]
    [InlineData("IEnumerable<int> x", "new object [] { new List<int>().AsReadOnly() }")]
    [InlineData("IEnumerable<char> x", @"new object [] { ""value"" }")]
    [InlineData("", @"new object[] { }")]
    [InlineData("", "new object[] { 1, 2 }.ToArray()")] // actual values known at runtime only so constructor analysys skipped
    [InlineData("int x, params string[] y", "new object[] { 1, \"foo\", \"foo\" }")]
    public override async Task ReportsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
    {
        var source = $@"using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {{typeof(Foo)}}, {invocationValues});
            var otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new[] {{typeof(Foo)}}, constructorArguments: {invocationValues});
            var yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: {invocationValues}, typesToProxy: new[] {{typeof(Foo)}});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithGenericArgument()
    {
        var source = @"using System;
using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{

    public class FooTests
    {
        public T Foo<T>() where T : class
        {
            var substitute =  (T)SubstitutionContext.Current.SubstituteFactory.Create(new Type[] {typeof(T)}, null);
            var otherSubstitute = (T)SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy: new Type[] {typeof(T)}, constructorArguments: null);
            var yetAnotherSubstitute = (T)SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments: null, typesToProxy: new Type[] {typeof(T)});
            return substitute;
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenParamsParametersNotProvided()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo) }, new object[] { 1 });
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenParamsParametersProvided()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

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
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo) }, new object[] { 1, 2, 3 });
        }
    }
}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount_AndParamsParameterDefined()
    {
        var source = @"using NSubstitute;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo) }, new object[] { 1 })|];
        }
    }
}";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.Create do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }
}