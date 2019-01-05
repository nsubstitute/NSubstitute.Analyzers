using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace NSubstitute.Analyzers.Tests.Shared.Extensibility
{
    public class CombinedInlineData : DataAttribute
    {
        private readonly object[] _data;

        public CombinedInlineData(params object[] data)
        {
            _data = data;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var attribute = testMethod.GetCustomAttribute<CombinatoryData>() ??
                            testMethod.DeclaringType.GetCustomAttribute<CombinatoryData>();

            return attribute.Data.Select(variation => new[] { variation }.Concat(_data).ToArray());
        }
    }
}