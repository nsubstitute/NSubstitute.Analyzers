using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IArgumentMatcherMisuseDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForMethodCall(string arg);

        Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForIndexerCall(string arg);
        
        Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedArgMethod(string arg);
    }
}