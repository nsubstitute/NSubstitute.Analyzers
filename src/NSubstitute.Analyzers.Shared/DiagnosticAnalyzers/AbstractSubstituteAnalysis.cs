using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    // TODO remove duplication
    public abstract class AbstractSubstituteAnalysis<TInvocationExpression>
        where TInvocationExpression : SyntaxNode
    {
        public ConstructorContext CollectConstructorContext(SubstituteContext<TInvocationExpression> substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            var accessibleConstructors = GetAccessibleConstructors(proxyTypeSymbol);
            var invocationParameterTypes = GetInvocationInfo(substituteContext);
            var possibleConstructors = invocationParameterTypes != null && accessibleConstructors != null
                ? accessibleConstructors.Where(ctor => ctor.Parameters.Length == invocationParameterTypes.Count)
                    .ToList()
                : null;

            return new ConstructorContext(
                proxyTypeSymbol,
                accessibleConstructors,
                possibleConstructors,
                invocationParameterTypes);
        }

        protected abstract IList<SyntaxNode> GetInvocationArguments(TInvocationExpression invocationExpression);

        protected abstract IList<SyntaxNode> GetParameterExpressionsFromArrayArgument(SyntaxNode syntaxNode);

        private IList<ITypeSymbol> GetInvocationInfo(SubstituteContext<TInvocationExpression> substituteContext)
        {
            var infos = substituteContext.MethodSymbol.IsGenericMethod
                ? GetGenericInvocationArgumentTypes(substituteContext)
                : GetNonGenericInvocationArgumentTypes(substituteContext);

            return infos;
        }

        private IList<ITypeSymbol> GetGenericInvocationArgumentTypes(SubstituteContext<TInvocationExpression> substituteContext)
        {
            var arguments = GetInvocationArguments(substituteContext.InvocationExpression);

            if (arguments == null)
            {
                return null;
            }

            if (arguments.Count == 0)
            {
                return new List<ITypeSymbol>();
            }

            var typeInfos = arguments.Select(arg =>
                    substituteContext.SyntaxNodeAnalysisContext.SemanticModel
                        .GetTypeInfo(arg.DescendantNodes().First()))
                .ToList();

            var possibleParamsArgument = typeInfos.First();

            // if passing array of objects as a sole element
            if (arguments.Count == 1 &&
                possibleParamsArgument.ConvertedType is IArrayTypeSymbol arrayTypeSymbol &&
                arrayTypeSymbol.ElementType.Equals(substituteContext.SyntaxNodeAnalysisContext.Compilation.ObjectType))
            {
                return GetArgumentTypeInfo(substituteContext, arguments.First());
            }

            return typeInfos.Select(type => type.Type).ToList();
        }

        private IList<ITypeSymbol> GetNonGenericInvocationArgumentTypes(SubstituteContext<TInvocationExpression> substituteContext)
        {
            // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
            var arrayArgument = GetInvocationArguments(substituteContext.InvocationExpression)?.Skip(1).FirstOrDefault();
            if (arrayArgument == null)
            {
                return null;
            }

            return GetArgumentTypeInfo(substituteContext, arrayArgument);
        }

        private IList<ITypeSymbol> GetArgumentTypeInfo(SubstituteContext<TInvocationExpression> substituteContext, SyntaxNode arrayArgument)
        {
            var typeInfo =
                substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(arrayArgument.DescendantNodes()
                    .First());

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
                .ToList();

            return types;
        }

        private IList<IMethodSymbol> GetAccessibleConstructors(ITypeSymbol genericArgument)
        {
            var internalsVisibleToProxy = genericArgument.InternalsVisibleToProxyGenerator();

            return genericArgument.GetMembers().OfType<IMethodSymbol>().Where(symbol =>
                symbol.MethodKind == MethodKind.Constructor &&
                symbol.IsStatic == false &&
                (symbol.DeclaredAccessibility == Accessibility.Protected ||
                 symbol.DeclaredAccessibility == Accessibility.Public ||
                 (internalsVisibleToProxy && symbol.DeclaredAccessibility == Accessibility.Internal))).ToList();
        }

        private TypeInfo GetTypeInfo(SubstituteContext<TInvocationExpression> substituteContext, SyntaxNode syntax)
        {
            return substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(syntax);
        }
    }
}