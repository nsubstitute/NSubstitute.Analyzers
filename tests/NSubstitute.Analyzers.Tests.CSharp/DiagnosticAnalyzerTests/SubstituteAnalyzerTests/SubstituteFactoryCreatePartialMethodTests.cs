using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests
{
    public class SubstituteFactoryCreatePartialMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReportsDiagnostic_WhenUsedForInterface()
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(IFoo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use SubstitutionContext.Current.SubstituteFactory.Create(new Type[] { typeof(IFoo)}, null) instead of SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(IFoo)}, null) here.");
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenUsedForDelegate()
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Func<int>)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use SubstitutionContext.Current.SubstituteFactory.Create(new Type[] { typeof(Func<int>)}, null) instead of SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Func<int>)}, null) here.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        public override async Task ReportsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        public override async Task ReportsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        public override async Task ReportsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied()
        {
            var source = @"using System;
using NSubstitute.Core;
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied()
        {
            var source = @"using System;
using NSubstitute.Core;
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, new object[] { 1, 2, 3})|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, new object[] { 1 })|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            var source = @"using System;
using NSubstitute;
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, new object[] { 1 })|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            var source = @"using System;
using NSubstitute.Core;

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes)
        {
            var source = $@"using System;
using System.Runtime.CompilerServices;
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] {{ typeof(Foo)}}, null);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes)
        {
            var source = $@"using System;
using System.Runtime.CompilerServices;
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] {{ typeof(Foo)}}, null)|];
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {{typeof(Foo)}}, new object[] {{{invocationValues}}})|];
        }}
    }}
}}";
            await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
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
        [InlineData("int x, params string[] y", "new object[] { 1, \"foo\" }")]
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {{typeof(Foo)}}, {invocationValues});
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
            return (T) SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] {typeof(T)}, null);
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new [] { typeof(Foo) }, new object[] { 1 });
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new [] { typeof(Foo) }, new object[] { 1, 2, 3 });
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new [] { typeof(Foo) }, new object[] { 1 })|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }
    }
}