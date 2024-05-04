using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class CallArgument
{
    public IArgumentOperation? ArgumentOperation { get; }

    public ITypeSymbol ParameterTypeSymbol { get; }

    public ITypeSymbol ArgumentTypeSymbol { get; }

    public CallArgument(IArgumentOperation argumentOperation)
        : this(
            argumentOperation.Parameter.Type,
            argumentOperation.GetTypeSymbol())
    {
        this.ArgumentOperation = argumentOperation;
    }

    public CallArgument(ITypeSymbol parameterTypeSymbol, ITypeSymbol argumentTypeSymbol)
    {
        this.ParameterTypeSymbol = parameterTypeSymbol;
        this.ArgumentTypeSymbol = argumentTypeSymbol;
    }
}