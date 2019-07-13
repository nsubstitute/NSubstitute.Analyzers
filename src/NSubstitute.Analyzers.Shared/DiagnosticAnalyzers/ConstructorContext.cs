using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal struct ConstructorContext
    {
        public IMethodSymbol[] AccessibleConstructors { get; }

        public IMethodSymbol[] PossibleConstructors { get; }

        public ITypeSymbol[] InvocationParameters { get; }

        public ITypeSymbol ConstructorType { get; }

        public ConstructorContext(
            ITypeSymbol constructorType,
            IMethodSymbol[] accessibleConstructors,
            IMethodSymbol[] possibleConstructors,
            ITypeSymbol[] invocationParameters)
        {
            ConstructorType = constructorType;
            InvocationParameters = invocationParameters;
            AccessibleConstructors = accessibleConstructors;
            PossibleConstructors = possibleConstructors;
        }
    }
}