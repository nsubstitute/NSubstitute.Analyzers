using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NSubstitute.Analyzers.Tests.Shared.Extensibility
{
    public class CombinatoryTheoryDiscoverer : TheoryDiscoverer
    {
        public CombinatoryTheoryDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, object[] dataRow)
        {
            var attribute = testMethod.Method.GetCustomAttributes(typeof(CombinatoryData)).SingleOrDefault() ??
                            testMethod.TestClass.Class.GetCustomAttributes(typeof(CombinatoryData)).SingleOrDefault();

            var newDataRows = attribute.GetNamedArgument<object[]>(nameof(CombinatoryData.Data)).Select(variation => new[] { variation }.Concat(dataRow).ToArray());

            foreach (var row in newDataRows)
            {
                foreach (var xunitTestCase in base.CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, row))
                {
                    yield return xunitTestCase;
                }
            }
        }
    }
}