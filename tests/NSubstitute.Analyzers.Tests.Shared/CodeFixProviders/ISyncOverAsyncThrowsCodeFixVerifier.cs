using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface ISyncOverAsyncThrowsCodeFixVerifier
{
    Task ReplacesThrowsWithReturns_WhenUsedInMethod(string method, string updatedMethod);

    Task ReplacesThrowsWithReturns_WhenUsedInProperty(string method, string updatedMethod);

    Task ReplacesThrowsWithReturns_WhenUsedInIndexer(string method, string updatedMethod);
}