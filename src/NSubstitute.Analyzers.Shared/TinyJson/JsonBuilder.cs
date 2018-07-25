using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace NSubstitute.Analyzers.Shared.TinyJson
{
    [ExcludeFromCodeCoverage]
    internal class JsonBuilder
    {
        private StringBuilder _builder = new StringBuilder();
        private bool _pretty = false;
        private int _level;

        public JsonBuilder()
        {
        }

        public JsonBuilder(bool pretty)
        {
            this._pretty = pretty;
        }

        public void AppendBeginObject()
        {
            _level++;
            _builder.Append("{");
            if (_pretty)
            {
                AppendPrettyLineBreak();
            }
        }

        public void AppendEndObject()
        {
            _level--;
            if (_pretty)
            {
                RemovePrettyLineBreak();
            }

            if (_pretty)
            {
                AppendPrettyLineBreak();
            }

            _builder.Append("}");
            if (_pretty)
            {
                AppendPrettyLineBreak();
            }
        }

        public void AppendBeginArray()
        {
            _level++;
            _builder.Append("[");
            if (_pretty)
            {
                AppendPrettyLineBreak();
            }
        }

        public void AppendEndArray()
        {
            _level--;
            if (_pretty)
            {
                RemovePrettyLineBreak();
            }

            if (_pretty)
            {
                AppendPrettyLineBreak();
            }

            _builder.Append("]");
            if (_pretty)
            {
                AppendPrettyLineBreak();
            }
        }

        public void AppendSeperator()
        {
            if (_pretty)
            {
                RemovePrettyLineBreak();
            }

            _builder.Append(",");
            if (_pretty)
            {
                AppendPrettyLineBreak();
            }
        }

        public void AppendNull()
        {
            _builder.Append("null");
        }

        public void AppendBool(bool b)
        {
            _builder.Append(b ? "true" : "false");
        }

        public void AppendNumber(object number)
        {
            if (number != null)
            {
                string numberString = number.ToString();
                if (number.GetType().IsFloatingPoint(number))
                {
                    numberString = numberString.Replace(',', '.');
                    if (!numberString.Contains("."))
                    {
                        numberString += ".0";
                    }
                }

                _builder.Append(numberString);
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendString(string str)
        {
            if (str != null)
            {
                _builder.Append('\"');
                foreach (var c in str)
                {
                    switch (c)
                    {
                        case '"':
                            _builder.Append("\\\"");
                            break;
                        case '\\':
                            _builder.Append("\\\\");
                            break;
                        case '\b':
                            _builder.Append("\\b");
                            break;
                        case '\f':
                            _builder.Append("\\f");
                            break;
                        case '\n':
                            _builder.Append("\\n");
                            break;
                        case '\r':
                            _builder.Append("\\r");
                            break;
                        case '\t':
                            _builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if (_pretty || (codepoint >= 32 && codepoint <= 126))
                            {
                                _builder.Append(c);
                            }
                            else
                            {
                                _builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                            }

                            break;
                    }
                }

                _builder.Append('\"');
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendArray(IEnumerable enumerable)
        {
            if (enumerable != null)
            {
                AppendBeginArray();
                bool first = true;
                foreach (var item in enumerable)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        AppendSeperator();
                    }

                    AppendValue(item);
                }

                AppendEndArray();
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendDictionary(IDictionary dict)
        {
            if (dict != null)
            {
                AppendBeginObject();
                bool first = true;
                foreach (DictionaryEntry entry in dict)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        AppendSeperator();
                    }

                    AppendString(entry.Key.ToString());
                    _builder.Append(_pretty ? " : " : ":");
                    AppendValue(entry.Value);
                }

                AppendEndObject();
            }
            else
            {
                AppendNull();
            }
        }

        public void AppendValue(object value)
        {
            if (value == null)
            {
                AppendNull();
            }
            else if (value is bool)
            {
                AppendBool((bool)value);
            }
            else if (value is string)
            {
                AppendString((string)value);
            }
            else if (value is char)
            {
                AppendString(string.Empty + value);
            }
            else if (IsEnum(value))
            {
                AppendNumber((int)value);
            }
            else if (IsNumber(value))
            {
                AppendNumber(value);
            }
        }

        public void AppendName(string name)
        {
            AppendString(name);
            _builder.Append(_pretty ? " : " : ":");
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        internal static bool IsNumber(object value)
        {
            return value != null && value.GetType().IsNumeric(value);
        }

        internal static bool IsEnum(object value)
        {
            return value != null && value.GetType().GetTypeInfo().IsEnum;
        }

        internal static bool IsSupported(object obj)
        {
            switch (obj)
            {
                case null:
                case bool _:
                case string _:
                case char _:
                    return true;
            }

            return IsEnum(obj) || IsNumber(obj);
        }

        private bool HasPrettyLineBreak()
        {
            return _builder.ToString().EndsWith("\t", StringComparison.Ordinal) ||
                   _builder.ToString().EndsWith("\n", StringComparison.Ordinal);
        }

        private void RemovePrettyLineBreak()
        {
            while (HasPrettyLineBreak())
            {
                _builder.Remove(_builder.Length - 1, 1);
            }
        }

        private void AppendPrettyLineBreak()
        {
            _builder.Append("\n");
            for (int i = 0; i < _level; i++)
            {
                _builder.Append("\t");
            }
        }
    }
}