using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace NSubstitute.Analyzers.Shared.TinyJson;

[ExcludeFromCodeCoverage]
internal class JsonParser : IDisposable
{
    private enum Token
    {
        None,
        CurlyOpen,
        CurlyClose,
        SquareOpen,
        SquareClose,
        Colon,
        Comma,
        String,
        Number,
        BoolOrNull
    }

    private StringReader _json;

    // temporary allocated
    private StringBuilder _sb = new StringBuilder();

    public static object ParseValue(string jsonString)
    {
        using (var parser = new JsonParser(jsonString))
        {
            return parser.ParseValue();
        }
    }

    internal JsonParser(string jsonString)
    {
        _json = new StringReader(jsonString);
    }

    public void Dispose()
    {
        _json.Dispose();
        _json = null;
    }

    // ** Reading Token **//
    private bool EndReached()
    {
        return _json.Peek() == -1;
    }

    private bool PeekWordbreak()
    {
        var c = PeekChar();
        return c == ' ' || c == ',' || c == ':' || c == '\"' || c == '{' || c == '}' || c == '[' || c == ']' ||
               c == '\t' || c == '\n' || c == '\r';
    }

    private bool PeekWhitespace()
    {
        var c = PeekChar();
        return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    private char PeekChar()
    {
        return Convert.ToChar(_json.Peek());
    }

    private char ReadChar()
    {
        return Convert.ToChar(_json.Read());
    }

    private string ReadWord()
    {
        _sb.Clear();
        while (!PeekWordbreak() && !EndReached())
        {
            _sb.Append(ReadChar());
        }

        return EndReached() ? null : _sb.ToString();
    }

    private void EatWhitespace()
    {
        while (PeekWhitespace())
        {
            _json.Read();
        }
    }

    private Token PeekToken()
    {
        EatWhitespace();
        if (EndReached())
            return Token.None;
        switch (PeekChar())
        {
            case '{':
                return Token.CurlyOpen;
            case '}':
                return Token.CurlyClose;
            case '[':
                return Token.SquareOpen;
            case ']':
                return Token.SquareClose;
            case ',':
                return Token.Comma;
            case '"':
                return Token.String;
            case ':':
                return Token.Colon;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case '-':
                return Token.Number;
            case 't':
            case 'f':
            case 'n':
                return Token.BoolOrNull;
            default:
                return Token.None;
        }
    }

    // ** Parsing Parts **//
    private object ParseBoolOrNull()
    {
        if (PeekToken() == Token.BoolOrNull)
        {
            var boolValue = ReadWord();
            if (boolValue == "true")
                return true;
            if (boolValue == "false")
                return false;
            if (boolValue == "null")
                return null;
            Debug.WriteLine("unexpected bool value: " + boolValue);
            return null;
        }
        else
        {
            Debug.WriteLine("unexpected bool token: " + PeekToken());
            return null;
        }
    }

    private object ParseNumber()
    {
        if (PeekToken() == Token.Number)
        {
            var number = ReadWord();
            if (number.Contains("."))
            {
                // Debug.WriteLine("parse floating point: " + number);
                double parsed;
                if (double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                    return parsed;
            }
            else
            {
                // Debug.WriteLine("parse integer: " + number);
                long parsed;
                if (long.TryParse(number, out parsed))
                    return parsed;
            }

            Debug.WriteLine("unexpected number value: " + number);
            return null;
        }
        else
        {
            Debug.WriteLine("unexpected number token: " + PeekToken());
            return null;
        }
    }

    private string ParseString()
    {
        if (PeekToken() == Token.String)
        {
            ReadChar(); // ditch opening quote

            _sb.Clear();
            char c;
            while (true)
            {
                if (EndReached())
                    return null;

                c = ReadChar();
                switch (c)
                {
                    case '"':
                        return _sb.ToString();
                    case '\\':
                        if (EndReached())
                            return null;

                        c = ReadChar();
                        switch (c)
                        {
                            case '"':
                            case '\\':
                            case '/':
                                _sb.Append(c);
                                break;
                            case 'b':
                                _sb.Append('\b');
                                break;
                            case 'f':
                                _sb.Append('\f');
                                break;
                            case 'n':
                                _sb.Append('\n');
                                break;
                            case 'r':
                                _sb.Append('\r');
                                break;
                            case 't':
                                _sb.Append('\t');
                                break;
                            case 'u':
                                var hex = string.Concat(ReadChar(), ReadChar(), ReadChar(), ReadChar());
                                _sb.Append((char)Convert.ToInt32(hex, 16));
                                break;
                        }

                        break;
                    default:
                        _sb.Append(c);
                        break;
                }
            }
        }
        else
        {
            Debug.WriteLine("unexpected string token: " + PeekToken());
            return null;
        }
    }

    // ** Parsing Objects **//
    private Dictionary<string, object> ParseObject()
    {
        if (PeekToken() == Token.CurlyOpen)
        {
            _json.Read(); // ditch opening brace

            var table = new Dictionary<string, object>();
            while (true)
            {
                switch (PeekToken())
                {
                    case Token.None:
                        return null;
                    case Token.Comma:
                        _json.Read();
                        continue;
                    case Token.CurlyClose:
                        _json.Read();
                        return table;
                    default:
                        var name = ParseString();
                        if (string.IsNullOrEmpty(name))
                            return null;

                        if (PeekToken() != Token.Colon)
                            return null;
                        _json.Read(); // ditch the colon

                        table[name] = ParseValue();
                        break;
                }
            }
        }
        else
        {
            Debug.WriteLine("unexpected object token: " + PeekToken());
            return null;
        }
    }

    private List<object> ParseArray()
    {
        if (PeekToken() == Token.SquareOpen)
        {
            _json.Read(); // ditch opening brace

            var array = new List<object>();
            while (true)
            {
                switch (PeekToken())
                {
                    case Token.None:
                        return null;
                    case Token.Comma:
                        _json.Read();
                        continue;
                    case Token.SquareClose:
                        _json.Read();
                        return array;
                    default:
                        array.Add(ParseValue());
                        break;
                }
            }
        }
        else
        {
            Debug.WriteLine("unexpected array token: " + PeekToken());
            return null;
        }
    }

    private object ParseValue()
    {
        switch (PeekToken())
        {
            case Token.String:
                return ParseString();
            case Token.Number:
                return ParseNumber();
            case Token.BoolOrNull:
                return ParseBoolOrNull();
            case Token.CurlyOpen:
                return ParseObject();
            case Token.SquareOpen:
                return ParseArray();
        }

        Debug.WriteLine("unexpected value token: " + PeekToken());
        return null;
    }
}