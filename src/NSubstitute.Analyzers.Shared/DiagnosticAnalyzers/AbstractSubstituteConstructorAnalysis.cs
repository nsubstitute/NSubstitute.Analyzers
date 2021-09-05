using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstituteConstructorAnalysis<TInvocationExpression, TArgumentSyntax> : ISubstituteConstructorAnalysis<TInvocationExpression> where TInvocationExpression : SyntaxNode
        where TArgumentSyntax : SyntaxNode
    {
        public ConstructorContext CollectConstructorContext(SubstituteContext<TInvocationExpression> substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            if (proxyTypeSymbol.Kind == SymbolKind.TypeParameter)
            {
                return new ConstructorContext(proxyTypeSymbol, null, null, null);
            }

            var accessibleConstructors = GetAccessibleConstructors(proxyTypeSymbol);
            var invocationParameterTypes = GetInvocationInfo(substituteContext);

            bool IsPossibleConstructor(IMethodSymbol methodSymbol)
            {
                var nonParamsParametersCount = methodSymbol.Parameters.Count(parameter => !parameter.IsParams);

                if (nonParamsParametersCount == methodSymbol.Parameters.Length)
                {
                    return methodSymbol.Parameters.Length == invocationParameterTypes.Length;
                }

                return invocationParameterTypes.Length >= nonParamsParametersCount;
            }

            var possibleConstructors = invocationParameterTypes != null && accessibleConstructors != null
                ? accessibleConstructors.Where(IsPossibleConstructor)
                    .ToArray()
                : null;

            return new ConstructorContext(
                proxyTypeSymbol,
                accessibleConstructors,
                possibleConstructors,
                invocationParameterTypes);
        }

        protected abstract IList<TArgumentSyntax> GetInvocationArguments(TInvocationExpression invocationExpression);

        protected abstract IList<SyntaxNode> GetParameterExpressionsFromArrayArgument(TArgumentSyntax syntaxNode);

        private ITypeSymbol[] GetInvocationInfo(SubstituteContext<TInvocationExpression> substituteContext)
        {
            var infos = substituteContext.MethodSymbol.IsGenericMethod
                ? GetGenericInvocationArgumentTypes(substituteContext)
                : GetNonGenericInvocationArgumentTypes(substituteContext);

            return infos;
        }

        private ITypeSymbol[] GetGenericInvocationArgumentTypes(SubstituteContext<TInvocationExpression> substituteContext)
        {
            var arguments = GetInvocationArguments(substituteContext.InvocationExpression);

            if (arguments == null)
            {
                return null;
            }

            if (arguments.Count == 0)
            {
                return Array.Empty<ITypeSymbol>();
            }

            var typeInfos = arguments.Select(arg => GetTypeInfo(substituteContext, arg.DescendantNodes().First()))
                .ToList();

            var possibleParamsArgument = typeInfos.First();

            // if passing array of objects as a sole element
            if (arguments.Count == 1 &&
                possibleParamsArgument.ConvertedType is IArrayTypeSymbol arrayTypeSymbol &&
                arrayTypeSymbol.ElementType.Equals(substituteContext.SyntaxNodeAnalysisContext.Compilation.ObjectType))
            {
                return GetArgumentTypeInfo(substituteContext, arguments.First());
            }

            return typeInfos.Select(type => type.Type).ToArray();
        }

        private ITypeSymbol[] GetNonGenericInvocationArgumentTypes(SubstituteContext<TInvocationExpression> substituteContext)
        {
            // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
            var arrayArgument = GetInvocationArguments(substituteContext.InvocationExpression)?.Skip(1).FirstOrDefault();
            if (arrayArgument == null)
            {
                return null;
            }

            return GetArgumentTypeInfo(substituteContext, arrayArgument);
        }

        private ITypeSymbol[] GetArgumentTypeInfo(SubstituteContext<TInvocationExpression> substituteContext, TArgumentSyntax arrayArgument)
        {
            var typeInfo = GetTypeInfo(substituteContext, arrayArgument.DescendantNodes().First());

            if (typeInfo.ConvertedType != null &&
                typeInfo.ConvertedType.TypeKind == TypeKind.Array &&
                typeInfo.Type == null)
            {
                return null;
            }

            // new object[] { }; // means we dont pass any arguments
            var parameterExpressionsFromArrayArgument = GetParameterExpressionsFromArrayArgument(arrayArgument);
            if (parameterExpressionsFromArrayArgument == null)
            {
                return null;
            }

            // new object[] { 1, 2, 3}); // means we pass arguments
            var types = parameterExpressionsFromArrayArgument
                .Select(exp => GetTypeInfo(substituteContext, exp).Type)
                .ToArray();

            return types;
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

        private TypeInfo GetTypeInfo(SubstituteContext<TInvocationExpression> substituteContext, SyntaxNode syntax)
        {
            return substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(syntax);
        }
    }
}