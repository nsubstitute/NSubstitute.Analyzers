﻿using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.SubstituteForInternalMemberCodeFixProviderTests
{
    public class ForPartsOfMethodTests : SubstituteForInternalMemberCodeFixVerifier
    {
        [Fact]
        public override async Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass()
        {
            var oldSource = @"Imports NSubstitute

Namespace MyNamespace
    Namespace MyInnerNamespace
        Friend Class Foo
        End Class

        Public Class FooTests
            Public Sub Test()
                Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
            End Sub
        End Class
    End Namespace
End Namespace
";
            var newSource = @"Imports NSubstitute

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Namespace MyInnerNamespace
        Friend Class Foo
        End Class

        Public Class FooTests
            Public Sub Test()
                Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
            End Sub
        End Class
    End Namespace
End Namespace
";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass()
        {
            var oldSource = @"Imports NSubstitute.Core

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            var newSource = @"Imports NSubstitute.Core

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty()
        {
            var oldSource = @"Imports System.Reflection
Imports NSubstitute.Core
<Assembly: AssemblyVersion(""1.0.0"")>
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            var newSource = @"Imports System.Reflection
Imports NSubstitute.Core
<Assembly: AssemblyVersion(""1.0.0"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass()
        {
            var oldSource = @"Imports NSubstitute.Core

Namespace MyNamespace
    Friend Class Foo
        Friend Class Bar
        End Class
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo.Bar)()
        End Sub
    End Class
End Namespace
";
            var newSource = @"Imports NSubstitute.Core

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Friend Class Foo
        Friend Class Bar
        End Class
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo.Bar)()
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass()
        {
            var oldSource = @"Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, oldSource);
        }

        [Fact]
        public override async Task DoesNot_AppendsInternalsVisibleTo_WhenInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            var oldSource = @"Imports NSubstitute.Core

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherFirstAssembly"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherSecondAssembly"")>
Namespace MyNamespace
    Friend Class Foo
        Friend Class Bar
        End Class
    End Class

    Public Class FooTests
        Public Sub Test()
            NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace";

            await VerifyFix(oldSource, oldSource);
        }
    }
}