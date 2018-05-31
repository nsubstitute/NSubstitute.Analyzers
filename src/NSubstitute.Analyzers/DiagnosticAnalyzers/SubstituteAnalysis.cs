using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Extensions;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers.DiagnosticAnalyzers
{
    // TODO remove duplication
    public static class SubstituteAnalysis
    {
        public static ConstructorContext CollectConstructorContext(SubstituteContext substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            var accessibleConstructors = GetAccessibleConstructors(proxyTypeSymbol);
            var invocationParameterTypes = GetInvocationInfo(substituteContext);
            var possibleConstructors = invocationParameterTypes != null && accessibleConstructors != null
                ? accessibleConstructors.Where(ctor => ctor.Parameters.Length == invocationParameterTypes.Count)
                    .ToList().AsReadOnly()
                : null;

            return new ConstructorContext(
                proxyTypeSymbol,
                accessibleConstructors,
                possibleConstructors,
                invocationParameterTypes);
        }

        public static bool InternalsVisibleToProxyGenerator(ISymbol typeSymbol)
        {
            var internalsVisibleToAttribute = typeSymbol.ContainingAssembly.GetAttributes()
                .FirstOrDefault(att =>
                    att.AttributeClass.ToString() == MetadataNames.InternalsVisibleToAttributeFullTypeName);

            return internalsVisibleToAttribute != null &&
                   internalsVisibleToAttribute.ConstructorArguments.Any(arg =>
                       arg.Value.ToString() == MetadataNames.CastleDynamicProxyGenAssembly2Name);
        }

        private static IList<ITypeSymbol> GetInvocationInfo(SubstituteContext substituteContext)
        {
            var infos = substituteContext.MethodSymbol.IsGenericMethod
                ? GetGenericInvocationArgumentTypes(substituteContext)
                : GetNonGenericInvocationArgumentTypes(substituteContext);

            return infos;
        }

        private static IList<ITypeSymbol> GetGenericInvocationArgumentTypes(SubstituteContext substituteContext)
        {
            if (substituteContext.InvocationExpression.ArgumentList == null)
            {
                return null;
            }

            var arguments = substituteContext.InvocationExpression.ArgumentList.Arguments;

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

        private static IList<ITypeSymbol> GetNonGenericInvocationArgumentTypes(SubstituteContext substituteContext)
        {
            // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
            var arrayArgument = substituteContext.InvocationExpression.ArgumentList?.Arguments.Skip(1).FirstOrDefault();
            if (arrayArgument == null)
            {
                return null;
            }

            return GetArgumentTypeInfo(substituteContext, arrayArgument);
        }

        private static IList<ITypeSymbol> GetArgumentTypeInfo(SubstituteContext substituteContext, ArgumentSyntax arrayArgument)
        {
            var typeInfo = substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(arrayArgument.DescendantNodes().First());

            if (typeInfo.ConvertedType != null &&
                typeInfo.ConvertedType.TypeKind == TypeKind.Array &&
                typeInfo.Type == null)
            {
                return new List<ITypeSymbol>();
            }

            // new object[] { }; // means we dont pass any arguments
            var parameterExpressionsFromArrayArgument =
                arrayArgument.GetArgumentExpression().GetParameterExpressionsFromArrayArgument();
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

        private static IList<IMethodSymbol> GetAccessibleConstructors(ITypeSymbol genericArgument)
        {
            var internalsVisibleToProxy = InternalsVisibleToProxyGenerator(genericArgument);

            return genericArgument.GetMembers().OfType<IMethodSymbol>().Where(symbol =>
                symbol.MethodKind == MethodKind.Constructor &&
                symbol.IsStatic == false &&
                (symbol.DeclaredAccessibility == Accessibility.Protected ||
                 symbol.DeclaredAccessibility == Accessibility.Public ||
                 (internalsVisibleToProxy && symbol.DeclaredAccessibility == Accessibility.Internal))).ToList();
        }

        private static TypeInfo GetTypeInfo(SubstituteContext substituteContext, SyntaxNode syntax)
        {
            return substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(syntax);
        }
    }
}