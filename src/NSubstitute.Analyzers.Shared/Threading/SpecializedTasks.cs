using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Shared.Threading
{
    internal class SpecializedTasks
    {
        // no Task.CompletedTask in netstandard1.1
        internal static Task CompletedTask { get; } = Task.FromResult(default(object));
    }
}