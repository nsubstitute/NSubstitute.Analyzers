using System;
using System.Reflection;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class EnumExtensions
{
    public static string GetDisplayName(this Enum @enum)
    {
        var field = @enum.GetType().GetTypeInfo().GetDeclaredField(@enum.ToString());
        return field.GetCustomAttribute<DisplayNameAttribute>().Name;
    }
}