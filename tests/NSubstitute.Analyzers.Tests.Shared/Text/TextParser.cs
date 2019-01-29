// https://github.com/JosefPihrt/Roslynator/blob/master/src/Tests/TestFramework.Core/Text/TextParser.cs

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NSubstitute.Analyzers.Tests.Shared.Text
{
    public abstract class TextParser
    {
        public static TextParser Default { get; } = new DefaultTextParser();

        public abstract TextParserResult GetSpans(string s, bool reverse = false);

        private class DefaultTextParser : TextParser
        {
            private const string OpenToken = "[|";
            private const string CloseToken = "|]";
            private const string OpenCloseTokens = OpenToken + CloseToken;
            private const int TokensLength = 4;

            public override TextParserResult GetSpans(string s, bool reverse = false)
            {
                var sb = new StringBuilder(s.Length - TokensLength);

                var startPending = false;
                LinePositionInfo start = default;
                Stack<LinePositionInfo> stack = null;
                List<LinePositionSpanInfo> spans = null;

                var lastPos = 0;

                var line = 0;
                var column = 0;

                var length = s.Length;

                var i = 0;
                while (i < length)
                {
                    switch (s[i])
                    {
                        case '\r':
                            {
                                if (PeekNextChar() == '\n')
                                {
                                    i++;
                                }

                                line++;
                                column = 0;
                                i++;
                                continue;
                            }

                        case '\n':
                            {
                                line++;
                                column = 0;
                                i++;
                                continue;
                            }

                        case '[':
                            {
                                var nextChar = PeekNextChar();
                                if (nextChar == '|')
                                {
                                    sb.Append(s, lastPos, i - lastPos);

                                    var start2 = new LinePositionInfo(sb.Length, line, column);

                                    if (stack != null)
                                    {
                                        stack.Push(start2);
                                    }
                                    else if (!startPending)
                                    {
                                        start = start2;
                                        startPending = true;
                                    }
                                    else
                                    {
                                        stack = new Stack<LinePositionInfo>();
                                        stack.Push(start);
                                        stack.Push(start2);
                                        startPending = false;
                                    }

                                    i += 2;
                                    lastPos = i;
                                    continue;
                                }
                                else if (nextChar == '['
                                    && PeekChar(2) == '|'
                                    && PeekChar(3) == ']')
                                {
                                    i++;
                                    column++;
                                    CloseSpan();
                                    i += 3;
                                    lastPos = i;
                                    continue;
                                }

                                break;
                            }

                        case '|':
                            {
                                if (PeekNextChar() == ']')
                                {
                                    CloseSpan();
                                    i += 2;
                                    lastPos = i;
                                    continue;
                                }

                                break;
                            }
                    }

                    column++;
                    i++;
                }

                if (startPending
                    || stack?.Count > 0)
                {
                    throw new InvalidOperationException();
                }

                sb.Append(s, lastPos, s.Length - lastPos);

                if (spans != null
                    && reverse)
                {
                    spans.Reverse();
                }

                return new TextParserResult(
                    sb.ToString(),
                    spans?.ToImmutableArray() ?? ImmutableArray<LinePositionSpanInfo>.Empty);

                char PeekNextChar()
                {
                    return PeekChar(1);
                }

                char PeekChar(int offset)
                {
                    return (i + offset >= s.Length) ? '\0' : s[i + offset];
                }

                void CloseSpan()
                {
                    if (stack != null)
                    {
                        start = stack.Pop();
                    }
                    else if (startPending)
                    {
                        startPending = false;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    var end = new LinePositionInfo(sb.Length + i - lastPos, line, column);

                    var span = new LinePositionSpanInfo(start, end);

                    (spans ?? (spans = new List<LinePositionSpanInfo>())).Add(span);

                    sb.Append(s, lastPos, i - lastPos);
                }
            }
        }
    }
}
