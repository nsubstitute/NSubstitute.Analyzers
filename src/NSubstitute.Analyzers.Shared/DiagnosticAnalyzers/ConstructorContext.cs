using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal struct ConstructorContext
    {
        public IList<IMethodSymbol> AccessibleConstructors { get; }

        public IList<IMethodSymbol> PossibleConstructors { get; }

        public IList<ITypeSymbol> InvocationParameters { get; }

        public ITypeSymbol ConstructorType { get; }

        public ConstructorContext(
            ITypeSymbol constructorType,
            IList<IMethodSymbol> accessibleConstructors,
            IList<IMethodSymbol> possibleConstructors,
            IList<ITypeSymbol> invocationParameters)
        {
            ConstructorType = constructorType;
            InvocationParameters = invocationParameters;
            AccessibleConstructors = accessibleConstructors;
            PossibleConstructors = possibleConstructors;
        }
    }
}