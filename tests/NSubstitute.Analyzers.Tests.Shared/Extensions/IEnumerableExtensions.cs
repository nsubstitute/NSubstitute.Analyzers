using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string ToDebugString(this IEnumerable<Diagnostic> diagnostics)
        {
            var s = string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString()));

            if (s.Length == 0)
            {
                s = "no diagnostics";
            }

            return $"{Environment.NewLine}Diagnostics:{Environment.NewLine}{s}{Environment.NewLine}";
        }
    }
}