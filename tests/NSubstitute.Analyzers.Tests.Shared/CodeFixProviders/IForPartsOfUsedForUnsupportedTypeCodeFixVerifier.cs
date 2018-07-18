using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface IForPartsOfUsedForUnsupportedTypeCodeFixVerifier
    {
        Task ReplacesForPartsOf_WithFor_WhenUsedWithInterface();

        Task ReplacesForPartsOf_WithFor_WhenUsedWithDelegate();
    }
}