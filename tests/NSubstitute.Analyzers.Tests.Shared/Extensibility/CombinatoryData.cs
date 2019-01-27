using System;

namespace NSubstitute.Analyzers.Tests.Shared.Extensibility
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CombinatoryData : Attribute
    {
        internal object[] Data { get; }

        public CombinatoryData(params object[] data)
        {
            Data = data;
        }
    }
}