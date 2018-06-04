using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Tests.Shared
{
    public class AnalyzersConventionFixture
    {
        public void AssertDiagnosticAnalyzerAttributeUsageFormAssemblyContaining<T>(string expectedLanguage)
        {
            AssertDiagnosticAnalyzerAttributeUsageFormAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertDiagnosticAnalyzerAttributeUsageFormAssemblyContaining(Type type, string expectedLanguage)
        {
            var diagnosticAnalyzerType = typeof(DiagnosticAnalyzer);
            var types = type.Assembly.GetTypes().Where(innerType => diagnosticAnalyzerType.IsAssignableFrom(innerType)).ToList();

            types.Should().OnlyContain(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(true).Count() == 1, "because each analyzer should be marked with only one attribute DiagnosticAnalyzerAttribute");
            types.SelectMany(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(true)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each analyzer should support only selected language ${expectedLanguage}");
        }
    }
}