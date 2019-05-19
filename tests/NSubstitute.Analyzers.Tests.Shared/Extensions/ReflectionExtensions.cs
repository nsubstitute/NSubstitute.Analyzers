using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> GetTypesAssignableTo<T>(this Assembly assembly)
        {
            var type = typeof(T);
            return assembly.GetTypes().Where(innerType => type.IsAssignableFrom(innerType)).ToList();
        }
    }
}