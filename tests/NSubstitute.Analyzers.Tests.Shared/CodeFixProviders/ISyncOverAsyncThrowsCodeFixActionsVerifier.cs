using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface ISyncOverAsyncThrowsCodeFixActionsVerifier
{
    Task CreatesCodeAction_WhenOverloadSupported(string method, string expectedCodeActionTitle);

    Task CreatesCodeAction_ForModernSyntax(string method, string expectedCodeActionTitle);

    Task DoesNotCreateCodeAction_WhenOverloadNotSupported(string method);
}