using System;
using Xunit;
using Xunit.Sdk;

namespace NSubstitute.Analyzers.Tests.Shared.Extensibility;

[XunitTestCaseDiscoverer("NSubstitute.Analyzers.Tests.Shared.Extensibility.CombinatoryTheoryDiscoverer", "NSubstitute.Analyzers.Tests.Shared")]
[AttributeUsage(AttributeTargets.Method)]
public class CombinatoryTheory : TheoryAttribute
{
}