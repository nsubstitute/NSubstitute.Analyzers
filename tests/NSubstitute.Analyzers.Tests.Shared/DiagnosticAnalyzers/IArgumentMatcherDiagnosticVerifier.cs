using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IArgumentMatcherDiagnosticVerifier
    {
        Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg);

        Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg);

        Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg);
    }
}