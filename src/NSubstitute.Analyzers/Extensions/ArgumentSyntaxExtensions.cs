using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers.Extensions
{
    public static class ArgumentSyntaxExtensions
    {
        public static ExpressionSyntax GetArgumentExpression(this ArgumentSyntax argumentSyntax)
        {
#if CSHARP
            return argumentSyntax.Expression;
#elif VISUAL_BASIC
            return argumentSyntax.GetExpression();
#endif
        }
    }
}