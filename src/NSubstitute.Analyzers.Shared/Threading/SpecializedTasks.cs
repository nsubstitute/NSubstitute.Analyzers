using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Shared.Threading
{
    public class SpecializedTasks
    {
        // no Task.CompletedTask in netstandard1.1
        internal static Task CompletedTask { get; } = Task.FromResult(default(object));
    }
}