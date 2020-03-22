using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.Fixtures
{
    public class AnalyzersConventionFixture
    {
        public void AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining<T>()
        {
            AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining(typeof(T));
        }

        public void AssertCodeFixProviderInheritanceFromAssemblyContaining<T>()
        {
            AssertCodeFixProviderInheritanceFromAssemblyContaining(typeof(T));
        }

        public void AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining(Type type)
        {
            var assembly = type.Assembly;
            var analyzers = GetDiagnosticAnalyzers(assembly).ToList();
            var abstractDiagnosticAnalyzerTypes = assembly.GetTypesAssignableTo<AbstractDiagnosticAnalyzer>().ToList();

            abstractDiagnosticAnalyzerTypes.Should().BeEquivalentTo(analyzers);
            analyzers.Should().OnlyContain(analyzer => analyzer.IsSealed);
        }

        public void AssertCodeFixProviderInheritanceFromAssemblyContaining(Type type)
        {
            var codeFixProviders = GetCodeFixProviders(type.Assembly);
            codeFixProviders.Should().OnlyContain(analyzer => analyzer.IsSealed);
        }

        public void AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var codeFixProviders = GetCodeFixProviders(type.Assembly).ToList();

            codeFixProviders.Should().OnlyContain(innerType => innerType.GetCustomAttributes<ExportCodeFixProviderAttribute>(false).Count() == 1, "because each code fix provider should be marked with only one attribute ExportCodeFixProviderAttribute");
            codeFixProviders.SelectMany(innerType => innerType.GetCustomAttributes<ExportCodeFixProviderAttribute>(false)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each code fix provider should support only selected language ${expectedLanguage}");
        }

        public void AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var analyzers = GetDiagnosticAnalyzers(type.Assembly).ToList();

            analyzers.Should().OnlyContain(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(false).Count() == 1, "because each analyzer should be marked with only one attribute DiagnosticAnalyzerAttribute");
            analyzers.SelectMany(innerType => innerType.GetCustomAttributes<DiagnosticAnalyzerAttribute>(false)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each analyzer should support only selected language ${expectedLanguage}");
        }

        public void AssertCodeRefactoringProviderInheritanceFromAssemblyContaining<T>()
        {
            AssertCodeRefactoringProviderInheritanceFromAssemblyContaining(typeof(T));
        }

        public void AssertCodeRefactoringProviderInheritanceFromAssemblyContaining(Type type)
        {
            var codeRefactoringProviders = GetCodeRefactoringProviders(type.Assembly);
            codeRefactoringProviders.Should().OnlyContain(analyzer => analyzer.IsSealed);
        }

        public void AssertExportCodeRefactoringProviderAttributeUsageFromAssemblyContaining<T>(string expectedLanguage)
        {
            AssertExportCodeRefactoringProviderAttributeUsageFromAssemblyContaining(typeof(T), expectedLanguage);
        }

        public void AssertExportCodeRefactoringProviderAttributeUsageFromAssemblyContaining(Type type, string expectedLanguage)
        {
            var codeRefactoringProviders = GetCodeRefactoringProviders(type.Assembly).ToList();

            codeRefactoringProviders.Should().OnlyContain(innerType => innerType.GetCustomAttributes<ExportCodeRefactoringProviderAttribute>(false).Count() == 1, "because each code refactoring provider should be marked with only one attribute ExportCodeRefactoringProviderAttribute");
            codeRefactoringProviders.SelectMany(innerType => innerType.GetCustomAttributes<ExportCodeRefactoringProviderAttribute>(false)).Should()
                .OnlyContain(
                    attr => attr.Languages.Length == 1 && attr.Languages.Count(lang => lang == expectedLanguage) == 1,
                    $"because each code refactoring provider should support only selected language ${expectedLanguage}");
        }

        private IEnumerable<Type> GetDiagnosticAnalyzers(Assembly assembly)
        {
            return assembly.GetTypesAssignableTo<DiagnosticAnalyzer>();
        }

        private IEnumerable<Type> GetCodeFixProviders(Assembly assembly)
        {
            return assembly.GetTypesAssignableTo<CodeFixProvider>();
        }

        private IEnumerable<Type> GetCodeRefactoringProviders(Assembly assembly)
        {
            return assembly.GetTypesAssignableTo<CodeRefactoringProvider>();
        }
    }
}