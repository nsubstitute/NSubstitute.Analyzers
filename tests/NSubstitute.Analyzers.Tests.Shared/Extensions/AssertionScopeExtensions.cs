using System;
using FluentAssertions.Execution;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions;

public static class AssertionScopeExtensions
{
    public static void Fail(this AssertionScope assertionScope, string message)
    {
        assertionScope.AddPreFormattedFailure(message);
        assertionScope.FailWith(string.Empty, Array.Empty<object>());
    }
}