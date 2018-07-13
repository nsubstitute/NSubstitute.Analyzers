using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : VisualBasicDiagnosticVerifier, IReEntrantReturnsSetupDiagnosticVerifier
    {
        public abstract Task ReturnsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall);

        public abstract Task ReturnsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall);

        public abstract Task ReturnsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall);

        public abstract Task ReturnsDiagnostic_ForNestedReEntrantCall();

        public abstract Task ReturnsDiagnostic_ForSpecificNestedReEntrantCall();

        public abstract Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall);

        public abstract Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall);

        public abstract Task ReturnsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }
    }
}