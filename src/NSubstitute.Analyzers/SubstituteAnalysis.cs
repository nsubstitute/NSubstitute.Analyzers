using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.CodeAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;
#endif
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Extensions;
#if VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

#endif

namespace NSubstitute.Analyzers
{
    // TODO remove duplication
    public static class SubstituteAnalysis
    {
        public static InvocationInfo GetInfocationInfo(SubstituteAnalyzer.SubstituteContext substituteContext)
        {
            var infos = substituteContext.MethodSymbol.IsGenericMethod
                ? GetGenericInvocationArgumentTypes(substituteContext)
                : GetNonGenericInvocationArgumentTypes(substituteContext);

            return new InvocationInfo(infos);
        }

        private static IList<TypeInfo> GetGenericInvocationArgumentTypes(
            SubstituteAnalyzer.SubstituteContext substituteContext)
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
            if (arguments.Count == 1 && possibleParamsArgument.ConvertedType is IArrayTypeSymbol arrayTypeSymbol &&
                arrayTypeSymbol.ElementType.Equals(substituteContext.SyntaxNodeAnalysisContext.Compilation.ObjectType))
            {
                if (possibleParamsArgument.Type == null)
                {
                    return new List<TypeInfo>();
                }

                var parameterExpressionsFromArrayArgument = arguments.First().GetArgumentExpression()
                    .GetParameterExpressionsFromArrayArgument();

                var types = parameterExpressionsFromArrayArgument
                    .Select(exp => GetTypeInfo(substituteContext, exp))
                    .ToList();

                return types;
            }

            return typeInfos;
        }

        private static IList<TypeInfo> GetNonGenericInvocationArgumentTypes(
            SubstituteAnalyzer.SubstituteContext substituteContext)
        {
            // Substitute.For(new [] { typeof(T) }, new object[] { 1, 2, 3}) // actual arguments reside in second arg
            var arrayArgument = substituteContext.InvocationExpression.ArgumentList?.Arguments.Skip(1).FirstOrDefault();
            if (arrayArgument == null)
            {
                return null;
            }

            // Substitute.For(new [] { typeof(T) }, null) // means we dont pass any arguments
            var typeInfo =
                substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(arrayArgument.DescendantNodes()
                    .First());
            if (typeInfo.ConvertedType != null && typeInfo.ConvertedType.TypeKind == TypeKind.Array &&
                typeInfo.Type == null)
            {
                return new List<TypeInfo>();
            }

            // Substitute.For(new [] { typeof(T)}, new object[] { }); // means we dont pass any arguments
            var parameterExpressionsFromArrayArgument =
                arrayArgument.GetArgumentExpression().GetParameterExpressionsFromArrayArgument();
            if (parameterExpressionsFromArrayArgument.Count == 0)
            {
                return new List<TypeInfo>();
            }

            // Substitute.For(new [] { typeof(T)}, new object[] { 1, 2, 3}); // means we pass arguments
            var types = parameterExpressionsFromArrayArgument
                .Select(exp => GetTypeInfo(substituteContext, exp))
                .ToList();

            return types;
        }

        private static TypeInfo GetTypeInfo(SubstituteAnalyzer.SubstituteContext substituteContext, SyntaxNode syntax)
        {
            return substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(syntax);
        }

        public class InvocationInfo
        {
            private IList<ITypeSymbol> _typeSymbols;

            public IList<ITypeSymbol> CapturedSymbols
            {
                get
                {
                    return _typeSymbols = _typeSymbols ??
                                          TypeInfos?.Select(info => info.Type).Where(type => type != null).ToList();
                }
            }

            public IList<TypeInfo> TypeInfos { get; }

            public bool AllTypesCapured => TypeInfos != null && TypeInfos.Count == CapturedSymbols.Count;

            public InvocationInfo(IList<TypeInfo> typeInfos)
            {
                TypeInfos = typeInfos;
            }
        }
    }
}