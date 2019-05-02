using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IArgumentMatcherMisuseDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForMethodCall(string arg);

        Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForIndexerCall(string arg);

        Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedArgMethod(string arg);

        Task ReportsDiagnostics_WhenUseTogetherWithUnfortunatelyNamedArgDoInvoke(string argDoInvoke, string arg);

        Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForMethodCall(string argDo, string arg);

        Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForIndexerCall(string argDo, string arg);

        Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForMethodCall(string argInvoke, string arg);

        Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForIndexerCall(string argInvoke, string arg);
    }
}