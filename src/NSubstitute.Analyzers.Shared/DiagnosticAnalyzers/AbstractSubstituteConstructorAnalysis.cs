using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstituteConstructorAnalysis<TInvocationExpression> : ISubstituteConstructorAnalysis<TInvocationExpression>
        where TInvocationExpression : SyntaxNode
    {
        public ConstructorContext CollectConstructorContext(SubstituteContext<TInvocationExpression> substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            if (proxyTypeSymbol.Kind == SymbolKind.TypeParameter)
            {
                return new ConstructorContext(proxyTypeSymbol, null, null, null);
            }

            var accessibleConstructors = GetAccessibleConstructors(proxyTypeSymbol);
            var invocationParameterTypes = GetInvocationInfo(substituteContext);
            var possibleConstructors = invocationParameterTypes != null && accessibleConstructors != null
                ? accessibleConstructors.Where(ctor => ctor.Parameters.Length == invocationParameterTypes.Length)
                    .ToArray()
                : null;

            return new ConstructorContext(
                proxyTypeSymbol,
                accessibleConstructors,
                possibleConstructors,
                invocationParameterTypes);
        }

        private ITypeSymbol[] GetInvocationInfo(SubstituteContext<TInvocationExpression> substituteContext)
        {
            var infos = substituteContext.MethodSymbol.IsGenericMethod
                ? GetGenericInvocationArgumentTypes(substituteContext)
                : GetNonGenericInvocationArgumentTypes(substituteContext);

            return infos;
        }

        private ITypeSymbol[] GetGenericInvocationArgumentTypes(SubstituteContext<TInvocationExpression> substituteContext)
        {
            var operation = substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetOperation(substituteContext.InvocationExpression);

            ImmutableArray<IArgumentOperation>? arguments = null;
            if (operation is IInvocationOperation invocationOperation)
            {
                arguments = invocationOperation.Arguments;
            }

            if (arguments.HasValue == false)
            {
                return null;
            }

            if (arguments.Value.Length == 0)
            {
                return Array.Empty<ITypeSymbol>();
            }

            // if passing array of objects as a sole element
            if (arguments.Value.Length == 1 && arguments.Value.Single().Value is IArrayCreationOperation arrayTypeSymbol)
            {
                return TypeSymbols(arrayTypeSymbol);
            }

            return arguments.Value.SelectMany(ArgType).ToArray();
        }

        private ITypeSymbol[] GetNonGenericInvocationArgumentTypes(SubstituteContext<TInvocationExpression> substituteContext)
        {
            // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
            var arrayArgument = GetInvocationArguments(substituteContext,  substituteContext.InvocationExpression).Skip(1).FirstOrDefault();
            if (arrayArgument == null)
            {
                return null;
            }

            // if passing array of objects as a sole element
            if (arrayArgument.Value is IArrayCreationOperation arrayTypeSymbol)
            {
                return TypeSymbols(arrayTypeSymbol);
            }

            return ArgType(arrayArgument);
        }

        private IMethodSymbol[] GetAccessibleConstructors(ITypeSymbol genericArgument)
        {
            var internalsVisibleToProxy = genericArgument.InternalsVisibleToProxyGenerator();

            bool IsAccessible(IMethodSymbol symbol)
            {
                return symbol.DeclaredAccessibility == Accessibility.Protected ||
                       symbol.DeclaredAccessibility == Accessibility.Public;
            }

            bool IsVisibleToProxy(IMethodSymbol symbol)
            {
                if (internalsVisibleToProxy == false)
                {
                    return false;
                }

                return symbol.DeclaredAccessibility == Accessibility.Internal ||
                       symbol.DeclaredAccessibility == Accessibility.ProtectedOrInternal;
            }

            return genericArgument.GetMembers().OfType<IMethodSymbol>().Where(symbol =>
                symbol.MethodKind == MethodKind.Constructor &&
                symbol.IsStatic == false &&
                (IsAccessible(symbol) || IsVisibleToProxy(symbol))).ToArray();
        }

        private IEnumerable<IArgumentOperation> GetInvocationArguments(SubstituteContext<TInvocationExpression> substituteContext, TInvocationExpression invocationExpression)
        {
            var invocationOperation = (IInvocationOperation)substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetOperation(invocationExpression);

            return invocationOperation.GetOrderedArgumentOperations();
        }

        private ITypeSymbol[] TypeSymbols(IArrayCreationOperation arrayInitializerOperation)
        {
            return arrayInitializerOperation.Initializer.ElementValues.Select(item =>
                    item is IConversionOperation conversionOperation
                        ? conversionOperation.Operand.Type
                        : item.Type)
                .ToArray();
        }

        private ITypeSymbol[] ArgType(IArgumentOperation x)
        {
            if (x.ArgumentKind == ArgumentKind.ParamArray)
            {
                var arrayInitializerOperation = x.Value as IArrayCreationOperation;

                return TypeSymbols(arrayInitializerOperation);
            }

            return Array.Empty<ITypeSymbol>();
        }
    }
}