using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Analyzers.Shared.TinyJson
{
    [ExcludeFromCodeCoverage]
    internal static class Json
    {
        public const string Version = "1.0";

        public static T Decode<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);
            var jsonObj = JsonParser.ParseValue(json);
            if (jsonObj == null)
                return default(T);
            return JsonMapper.DecodeJsonObject<T>(jsonObj);
        }

        public static string Encode(object value, bool pretty = false)
        {
            var builder = new JsonBuilder(pretty);
            JsonMapper.EncodeValue(value, builder);
            return builder.ToString();
        }
    }

    [ExcludeFromCodeCoverage]
    internal static class JsonExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null || !type.GetTypeInfo().IsPrimitive;
        }

        public static bool IsNumeric(this Type type, object value)
        {
            if (type.GetTypeInfo().IsEnum)
                return false;
            switch (GetTypeCode(value))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    return underlyingType != null && underlyingType.IsNumeric(value);
                default:
                    return false;
            }
        }

        public static bool IsFloatingPoint(this Type type, object value)
        {
            if (type.GetTypeInfo().IsEnum)
            {
                return false;
            }

            switch (GetTypeCode(value))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    return underlyingType != null && underlyingType.IsFloatingPoint(value);
                default:
                    return false;
            }
        }

        public static void Clear(this System.Text.StringBuilder sb)
        {
            sb.Length = 0;
        }

        public static bool IsInstanceOfGenericType(this Type type, Type genericType)
        {
            while (type != null)
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType)
                    return true;
                type = type.GetTypeInfo().BaseType;
            }

            return false;
        }

        public static bool HasGenericInterface(this Type type, Type genericInterface)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            var interfaceTest = new Predicate<Type>(i =>
                i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
            return interfaceTest(type) || type.GetTypeInfo().ImplementedInterfaces.Any(i => interfaceTest(i));
        }

        private static TypeCode GetTypeCode(object value)
        {
            if (value == null)
                return TypeCode.Empty;
            if (value is IConvertible convertible)
                return convertible.GetTypeCode();
            return TypeCode.Object;
        }
    }
}