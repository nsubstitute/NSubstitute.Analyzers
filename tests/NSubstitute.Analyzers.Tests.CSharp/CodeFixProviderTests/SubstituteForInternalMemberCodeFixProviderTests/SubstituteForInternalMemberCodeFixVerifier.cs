using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests
{
    public abstract class SubstituteForInternalMemberCodeFixVerifier : CSharpCodeFixVerifier, ISubstituteForInternalMemberCodeFixVerifier
    {
        public abstract Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass();

        public abstract Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass();

        public abstract Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty();

        public abstract Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass();

        public abstract Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass();

        public abstract Task DoesNot_AppendsInternalsVisibleTo_WhenInternalsVisibleToAppliedToDynamicProxyGenAssembly2();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new SubstituteForInternalMemberCodeFixProvider();
        }
    }
}