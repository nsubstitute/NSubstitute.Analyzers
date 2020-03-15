﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.InternalSetupSpecificationCodeFixProviderTests
{
    public abstract class InternalSetupSpecificationCodeFixProviderVerifier : CSharpSuppressDiagnosticSettingsVerifier, IInternalSetupSpecificationCodeFixProviderVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberAnalyzer();

        protected override CodeFixProvider CodeFixProvider { get; } = new InternalSetupSpecificationCodeFixProvider();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ChangesInternalToPublic_ForIndexer_WhenUsedWithInternalMember(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ChangesInternalToPublic_ForProperty_WhenUsedWithInternalMember(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ChangesInternalToPublic_ForMethod_WhenUsedWithInternalMember(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task AppendsProtectedInternal_ToIndexer_WhenUsedWithInternalMember(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task AppendsProtectedInternal_ToProperty_WhenUsedWithInternalMember(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task AppendsProtectedInternal_ToMethod_WhenUsedWithInternalMember(string method);

        [CombinatoryTheory]
        [InlineData(".Bar")]
        [InlineData(".FooBar()")]
        [InlineData("[0]")]
        public abstract Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalMember(string method, string call);
    }
}