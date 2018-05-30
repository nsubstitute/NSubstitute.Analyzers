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
        public static IList<TypeInfo> GetInvocationInfo(SubstituteAnalyzer.SubstituteContext substituteContext)
        {
            var infos = substituteContext.MethodSymbol.IsGenericMethod
                ? GetGenericInvocationArgumentTypes(substituteContext)
                : GetNonGenericInvocationArgumentTypes(substituteContext);

            return infos;
        }

        private static IList<TypeInfo> GetGenericInvocationArgumentTypes(SubstituteAnalyzer.SubstituteContext substituteContext)
        {
            if (substituteContext.InvocationExpression.ArgumentList == null)
            {
                return null;
            }

            var arguments = substituteContext.InvocationExpression.ArgumentList.Arguments;

            if (arguments.Count == 0)
            {
                return new List<TypeInfo>();
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

            return typeInfos;
        }

        private static IList<TypeInfo> GetNonGenericInvocationArgumentTypes(SubstituteAnalyzer.SubstituteContext substituteContext)
        {
            // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
            var arrayArgument = substituteContext.InvocationExpression.ArgumentList?.Arguments.Skip(1).FirstOrDefault();
            if (arrayArgument == null)
            {
                return null;
            }

            return GetArgumentTypeInfo(substituteContext, arrayArgument);
        }

        private static IList<TypeInfo> GetArgumentTypeInfo(SubstituteAnalyzer.SubstituteContext substituteContext, ArgumentSyntax arrayArgument)
        {
            var typeInfo = substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(arrayArgument.DescendantNodes().First());

            if (typeInfo.ConvertedType != null &&
                typeInfo.ConvertedType.TypeKind == TypeKind.Array &&
                typeInfo.Type == null)
            {
                return new List<TypeInfo>();
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
                .Select(exp => GetTypeInfo(substituteContext, exp))
                .ToList();

            return types;
        }

        private static TypeInfo GetTypeInfo(SubstituteAnalyzer.SubstituteContext substituteContext, SyntaxNode syntax)
        {
            return substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(syntax);
        }
    }
}