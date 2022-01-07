using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Decoder = System.Func<System.Type, object, object>;
using Encoder = System.Action<object, NSubstitute.Analyzers.Shared.TinyJson.JsonBuilder>;

namespace NSubstitute.Analyzers.Shared.TinyJson
{
    [ExcludeFromCodeCoverage]
    internal static class JsonMapper
    {
        private static Encoder genericEncoder;
        private static Decoder genericDecoder;
        private static Dictionary<Type, Encoder> encoders = new Dictionary<Type, Encoder>();
        private static Dictionary<Type, Decoder> decoders = new Dictionary<Type, Decoder>();

        internal static Encoder GenericEncoder { get => genericEncoder; set => genericEncoder = value; }

        internal static Decoder GenericDecoder { get => genericDecoder; set => genericDecoder = value; }

        internal static Dictionary<Type, Encoder> Encoders { get => encoders; set => encoders = value; }

        static JsonMapper()
        {
            RegisterDefaultEncoder();
            RegisterDefaultDecoder();
        }

        public static void RegisterDecoder<T>(Decoder decoder)
        {
            if (typeof(T) == typeof(object))
            {
                GenericDecoder = decoder;
            }
            else
            {
                decoders[typeof(T)] = decoder;
            }
        }

        public static void RegisterEncoder<T>(Encoder encoder)
        {
            if (typeof(T) == typeof(object))
            {
                GenericEncoder = encoder;
            }
            else
            {
                Encoders[typeof(T)] = encoder;
            }
        }

        public static Decoder GetDecoder(Type type)
        {
            if (decoders.ContainsKey(type))
            {
                return decoders[type];
            }

            foreach (var entry in decoders)
            {
                var baseType = entry.Key;
                if (baseType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    return entry.Value;
                }
            }

            return GenericDecoder;
        }

        public static Encoder GetEncoder(Type type)
        {
            if (Encoders.ContainsKey(type))
            {
                return Encoders[type];
            }

            foreach (var entry in Encoders)
            {
                var baseType = entry.Key;
                if (baseType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    return entry.Value;
                }
            }

            return GenericEncoder;
        }

        public static T DecodeJsonObject<T>(object jsonObj)
        {
            var decoder = GetDecoder(typeof(T));
            return (T)decoder(typeof(T), jsonObj);
        }

        public static void EncodeValue(object value, JsonBuilder builder)
        {
            if (JsonBuilder.IsSupported(value))
            {
                builder.AppendValue(value);
            }
            else
            {
                var encoder = GetEncoder(value.GetType());
                if (encoder != null)
                {
                    encoder(value, builder);
                }
            }
        }

        public static void EncodeNameValue(string name, object value, JsonBuilder builder)
        {
            builder.AppendName(UnwrapName(name));
            EncodeValue(value, builder);
        }

        public static string UnwrapName(string name)
        {
            if (name.StartsWith("<", StringComparison.Ordinal) && name.Contains(">"))
            {
                return name.Substring(
                    name.IndexOf("<", StringComparison.Ordinal) + 1,
                    name.IndexOf(">", StringComparison.Ordinal) - 1);
            }

            return name;
        }

        public static bool DecodeValue(object target, string name, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                foreach (var field in type.GetTypeInfo().DeclaredFields)
                {
                    if (name == UnwrapName(field.Name))
                    {
                        if (value != null)
                        {
                            var targetType = field.FieldType;
                            var decodedValue = DecodeValue(value, targetType);

                            if (decodedValue != null && targetType.GetTypeInfo()
                                    .IsAssignableFrom(decodedValue.GetType().GetTypeInfo()))
                            {
                                field.SetValue(target, decodedValue);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            field.SetValue(target, null);
                            return true;
                        }
                    }
                }

                type = type.GetTypeInfo().BaseType;
            }

            return false;
        }

        private static object ConvertValue(object value, Type type)
        {
            if (value != null)
            {
                var safeType = Nullable.GetUnderlyingType(type) ?? type;
                if (!type.GetTypeInfo().IsEnum)
                {
                    return Convert.ChangeType(value, safeType);
                }
                else
                {
                    if (value is string)
                    {
                        return Enum.Parse(type, (string)value);
                    }
                    else
                    {
                        return Enum.ToObject(type, value);
                    }
                }
            }

            return value;
        }

        private static object DecodeValue(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (JsonBuilder.IsSupported(value))
            {
                value = ConvertValue(value, targetType);
            }

            // use a registered decoder
            if (value != null && !targetType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                var decoder = GetDecoder(targetType);
                value = decoder(targetType, value);
            }

            if (value != null && targetType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                return value;
            }
            else
            {
                Debug.WriteLine("couldn't decode: " + targetType);
                return null;
            }
        }

        private static void RegisterDefaultEncoder()
        {
            // register generic encoder
            RegisterEncoder<object>((obj, builder) =>
            {
                // Debug.WriteLine("using generic encoder");
                builder.AppendBeginObject();
                var type = obj.GetType();
                var first = true;
                while (type != null)
                {
                    foreach (var field in type.GetTypeInfo().DeclaredFields.Where(field => field.IsStatic == false))
                    {
                        if (first)
                            first = false;
                        else
                            builder.AppendSeperator();
                        EncodeNameValue(field.Name, field.GetValue(obj), builder);
                    }

                    type = type.GetTypeInfo().BaseType;
                }

                builder.AppendEndObject();
            });

            // register IDictionary encoder
            RegisterEncoder<IDictionary>((obj, builder) =>
            {
                // Debug.WriteLine("using IDictionary encoder");
                builder.AppendBeginObject();
                var first = true;
                var dict = (IDictionary)obj;
                foreach (var key in dict.Keys)
                {
                    if (first)
                        first = false;
                    else
                        builder.AppendSeperator();
                    EncodeNameValue(key.ToString(), dict[key], builder);
                }

                builder.AppendEndObject();
            });

            // register IEnumerable support for all list and array types
            RegisterEncoder<IEnumerable>((obj, builder) =>
            {
                // Debug.WriteLine("using IEnumerable encoder");
                builder.AppendBeginArray();
                var first = true;
                foreach (var item in (IEnumerable)obj)
                {
                    if (first)
                        first = false;
                    else
                        builder.AppendSeperator();
                    EncodeValue(item, builder);
                }

                builder.AppendEndArray();
            });

            // register zulu date support
            RegisterEncoder<DateTime>((obj, builder) =>
            {
                var date = (DateTime)obj;
                var zulu = date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                builder.AppendString(zulu);
            });
        }

        private static void RegisterDefaultDecoder()
        {
            // register generic decoder
            RegisterDecoder<object>((type, jsonObj) =>
            {
                var instance = Activator.CreateInstance(type);

                if (jsonObj is IDictionary)
                {
                    foreach (DictionaryEntry item in (IDictionary)jsonObj)
                    {
                        var name = (string)item.Key;
                        if (!DecodeValue(instance, name, item.Value))
                        {
                            Debug.WriteLine("couldn't decode field \"" + name + "\" of " + type);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("unsupported json type: " +
                                      (jsonObj != null ? jsonObj.GetType().ToString() : "null"));
                }

                return instance;
            });

            // register IList support
            RegisterDecoder<IEnumerable>((type, jsonObj) =>
            {
                if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    if (jsonObj is IList)
                    {
                        var jsonList = (IList)jsonObj;
                        if (type.IsArray)
                        {
                            // Arrays
                            var elementType = type.GetElementType();
                            var nullable = elementType.IsNullable();
                            var array = Array.CreateInstance(elementType, jsonList.Count);
                            for (var i = 0; i < jsonList.Count; i++)
                            {
                                var value = DecodeValue(jsonList[i], elementType);
                                if (value != null || nullable)
                                    array.SetValue(value, i);
                            }

                            return array;
                        }
                        else if (type.GetTypeInfo().GenericTypeArguments.Length == 1)
                        {
                            // Generic List
                            var genericType = type.GetTypeInfo().GenericTypeArguments[0];
                            if (type.HasGenericInterface(typeof(IList<>)))
                            {
                                // IList
                                IList instance = null;
                                var nullable = genericType.IsNullable();
                                if (type != typeof(IList) &&
                                    typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                                {
                                    instance = Activator.CreateInstance(type) as IList;
                                }
                                else
                                {
                                    var genericListType = typeof(List<>).MakeGenericType(genericType);
                                    instance = Activator.CreateInstance(genericListType) as IList;
                                }

                                foreach (var item in jsonList)
                                {
                                    var value = DecodeValue(item, genericType);
                                    if (value != null || nullable)
                                        instance.Add(value);
                                }

                                return instance;
                            }
                            else if (type.HasGenericInterface(typeof(ICollection<>)))
                            {
                                // ICollection
                                var listType = type.IsInstanceOfGenericType(typeof(HashSet<>))
                                    ? typeof(HashSet<>)
                                    : typeof(List<>);
                                var constructedListType = listType.MakeGenericType(genericType);
                                var instance = Activator.CreateInstance(constructedListType);
                                var nullable = genericType.IsNullable();
                                var addMethodInfo = type.GetTypeInfo().GetDeclaredMethod("Add");
                                if (addMethodInfo != null)
                                {
                                    foreach (var item in jsonList)
                                    {
                                        var value = DecodeValue(item, genericType);
                                        if (value != null || nullable)
                                            addMethodInfo.Invoke(instance, new object[] { value });
                                    }

                                    return instance;
                                }
                            }

                            Debug.WriteLine("IEnumerable type not supported " + type);
                        }
                    }

                    if (jsonObj is Dictionary<string, object>)
                    {
                        // Dictionary
                        var jsonDict = (Dictionary<string, object>)jsonObj;
                        if (type.GetTypeInfo().GenericTypeArguments.Length == 2)
                        {
                            IDictionary instance = null;
                            var keyType = type.GetTypeInfo().GenericTypeArguments[0];
                            var genericType = type.GetTypeInfo().GenericTypeArguments[1];
                            var nullable = genericType.IsNullable();
                            if (type != typeof(IDictionary) &&
                                typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                            {
                                instance = Activator.CreateInstance(type) as IDictionary;
                            }
                            else
                            {
                                var genericDictType = typeof(Dictionary<,>).MakeGenericType(keyType, genericType);
                                instance = Activator.CreateInstance(genericDictType) as IDictionary;
                            }

                            foreach (var item in jsonDict)
                            {
                                Debug.WriteLine(item.Key + " = " + JsonMapper.DecodeValue(item.Value, genericType));
                                var value = DecodeValue(item.Value, genericType);
                                object key = item.Key;
                                if (keyType == typeof(int))
                                    key = int.Parse(item.Key);
                                if (value != null || nullable)
                                    instance.Add(key, value);
                            }

                            return instance;
                        }
                        else
                        {
                            Debug.WriteLine("unexpected type arguemtns");
                        }
                    }

                    if (jsonObj is Dictionary<int, object>)
                    {
                        // Dictionary
                        // convert int to string key
                        var jsonDict = new Dictionary<string, object>();
                        foreach (var keyValuePair in (Dictionary<int, object>)jsonObj)
                        {
                            jsonDict.Add(keyValuePair.Key.ToString(), keyValuePair.Value);
                        }

                        if (type.GetTypeInfo().GenericTypeArguments.Length == 2)
                        {
                            IDictionary instance = null;
                            var keyType = type.GetTypeInfo().GenericTypeArguments[0];
                            var genericType = type.GetTypeInfo().GenericTypeArguments[1];
                            var nullable = genericType.IsNullable();
                            if (type != typeof(IDictionary) &&
                                typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                            {
                                instance = Activator.CreateInstance(type) as IDictionary;
                            }
                            else
                            {
                                var genericDictType = typeof(Dictionary<,>).MakeGenericType(keyType, genericType);
                                instance = Activator.CreateInstance(genericDictType) as IDictionary;
                            }

                            foreach (var item in jsonDict)
                            {
                                Debug.WriteLine(item.Key + " = " + DecodeValue(item.Value, genericType));
                                var value = DecodeValue(item.Value, genericType);
                                if (value != null || nullable)
                                    instance.Add(Convert.ToInt32(item.Key), value);
                            }

                            return instance;
                        }
                        else
                        {
                            Debug.WriteLine("unexpected type arguemtns");
                        }
                    }
                }

                Debug.WriteLine("couldn't decode: " + type);
                return null;
            });
        }
    }
}