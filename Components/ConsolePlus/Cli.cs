﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Components.ConsolePlus
{
    public static class Cli
    {
        private const string nameHeader = "Name";

        private const string valueHeader = "Value";

        private static int maxNameLength;

        private static int maxValueLength;

        private static object _syncObject = new object();

        private static bool _useTrace = false;

        public static bool UseTrace
        {
            get { return Cli._useTrace; }
            set
            {
                Cli._useTrace = value;

                if (value)
                {
                    _write = x => Trace.Write(x);
                    _writeLine = x => Trace.WriteLine(x);
                }
                else
                {
                    _write = Console.Write;
                    _writeLine = Console.WriteLine;
                }
            }
        }

        private static Action<string> _write = Console.Write;

        private static Action<string> _writeLine = Console.WriteLine;

        static Cli()
        {
            try
            {
                SetLengths(Console.BufferWidth);
            }
            catch (IOException)
            {
                SetLengths(100);
            }
        }

        private static void SetLengths(int consoleWidth)
        {
            maxNameLength = (int)Math.Floor(((double)consoleWidth - 3) / 2);
            maxValueLength = (int)Math.Ceiling(((double)consoleWidth - 4) / 2);
        }

        /// <summary>
        /// Writes a format string to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="arg">The arguments.</param>
        [DebuggerStepThrough]
        public static void Write(string format, params object[] arg)
        {
            WriteCore(format, false, arg);
        }

        /// <summary>
        /// Writes a format string and a new line to the console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="arg">The arguments.</param>
        [DebuggerStepThrough]
        public static void WriteLine(string format, params object[] arg)
        {
            WriteCore(format, true, arg);
        }

        [DebuggerStepThrough]
        public static void WriteLine()
        {
            _writeLine("");
        }        

        public static void DumpTable(IEnumerable<KeyValuePair<string, string>> nameValuePairs)
        {
            if (nameValuePairs.Count() == 0)
            {
                return;
            }

            const int paddingSize = 0;
            var longestNameLength = nameValuePairs.OrderByDescending(x => x.Key.Length).First().Key.Length;

            if (longestNameLength < nameHeader.Length)
            {
                longestNameLength = nameHeader.Length;
            }
            else if (longestNameLength > maxNameLength)
            {
                longestNameLength = maxNameLength;
            }

            var longestValueLength = nameValuePairs.OrderByDescending(x => x.Value.Length).First().Value.Length;

            if (longestValueLength < valueHeader.Length)
            {
                longestValueLength = valueHeader.Length;
            }
            else if (longestValueLength > maxValueLength)
            {
                longestValueLength = maxValueLength;

                if (longestNameLength < maxNameLength)
                {
                    longestValueLength += maxNameLength - longestNameLength;
                }
            }

            var padding = new string(' ', paddingSize);

            var headerColor = "~White~~|DarkGray~";

            var totalLength = paddingSize * 4 + 3 + longestNameLength + longestValueLength;

            //var newLine = totalLength == Console.BufferWidth ? "" : "\r\n";
            var newLine = "\r\n";

            Func<string, string, bool, string> createRow = (name, value, header) =>
            {
                if (name.Length > maxNameLength)
                {
                    name = name.Remove(maxNameLength - 3) + "~Yellow~...~R~";
                }

                if (value.Length > longestValueLength)
                {
                    value = value.Remove(longestValueLength - 3) + "~Yellow~...~R~";
                }

                var delim = "║" + (header ? headerColor : "") + padding;
                var reset = (header ? "~R~" : "");
                return delim + name.PadRight(longestNameLength + paddingSize) + reset +
                    delim + value.PadRight(longestValueLength + paddingSize) + reset + "║";
            };

            var divider = new string('═', totalLength - 2).ToCharArray();
            var verticalRuleOffset = longestNameLength + paddingSize * 2;
            divider[verticalRuleOffset] = '╦';
            var hrTop = "╔" + new string(divider) + "╗";
            divider[verticalRuleOffset] = '╬';
            var hrMiddle = "╠" + new string(divider) + "╣";
            divider[verticalRuleOffset] = '╩';
            var hrBottom = "╚" + new string(divider) + "╝";

            var rows = nameValuePairs
                .Select(x => createRow(x.Key, x.Value, false))
                .Aggregate((x, y) => x + newLine + y);

            WriteLine(hrTop + newLine +
                createRow(nameHeader, valueHeader, true) + newLine +
                hrMiddle + newLine +
                rows + newLine +
                hrBottom);
        }

        private static void DumpDetails(IEnumerable<KeyValuePair<string, string>> nameValuePairs)
        {
            foreach (var nvp in nameValuePairs)
            {
                WriteLine(
                    "~{0}~~|{1}~{2}~R~\r\n{3}\r\n",
                    ConsoleColor.White,
                    ConsoleColor.Blue,
                    nvp.Key,
                    nvp.Value);
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> CreateTable(object obj, bool escapeLines)
        {
            Func<PropertyInfo, string> tryGet = x =>
            {
                try
                {
#if NET35
                    return (x.GetValue(obj, null) ?? "null").ToString();
#else
                    return (x.GetValue(obj) ?? "null").ToString();
#endif
                }
                catch (Exception e)
                {
                    return string.Format("Error getting value: {0}", e);
                }
            };
            var t = obj.GetType();
            return t
                .GetProperties()
                .Select(x => new KeyValuePair<string, string>(
                    x.Name,
                    escapeLines ?
                        tryGet(x).Replace("\r", "\\r").Replace("\n", "\\n") :
                        tryGet(x)
                ));
        }

        public static void Dump<T>(T obj, DumpMode mode)
        {
            var nvps = CreateTable(obj, mode == DumpMode.Table);

            if (mode == DumpMode.Table)
            {
                DumpTable(nvps);
            }
            else if (mode == DumpMode.Details)
            {
                DumpDetails(nvps);
            }
            else
            {
                throw new ArgumentException("Invalid mode.");
            }
        }

        public static void Dump<T>(T obj)
        {
            Dump(obj, DumpMode.Table);
        }

#if NET35
        public static void WriteHeader(string format)
        {
            WriteHeader(format, "");
        }

        public static void WriteHeader(string format, string style)
#else
        public static void WriteHeader(string text, string style = "")
#endif
        {
            var divider = new string('═', Console.BufferWidth - 3).ToCharArray();
            var hrTop = "╔" + new string(divider) + "╗\r\n";
            var hrMiddle = "║" + style + " " + text.PadRight(Console.BufferWidth - 4).Replace("~", "~~") + "~R~║\r\n";
            var hrBottom = "╚" + new string(divider) + "╝\r\n";
            Write(hrTop + hrMiddle + hrBottom);
        }

        public static void WriteSubheader(string text, string style = "")
        {
            Cli.WriteLine(
                "{0}  {1}{2}{3}",
                style,
                Cli.Escape(text),
                new string(' ', Console.BufferWidth - text.Length - 3),
                "~R~");
        }

        /// <summary>
        /// The core console write function.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="newLine">Determines whether to print the message with a new line or not.</param>
        /// <param name="arg">The arguments.</param>
        [DebuggerStepThrough]
        private static void WriteCore(string format, bool newLine, params object[] arg)
        {
            lock (_syncObject)
            {
                format = string.Format(format, arg);
                var state = CliLexerState.ReadingText;

                var buffer = new StringBuilder();

                for (int i = 0; i < format.Length; i++)
                {
                    var c = format[i];

                    if (state == CliLexerState.ReadingText)
                    {
                        if (c == '~')
                        {
                            state = CliLexerState.ReadingToken;

                            if (buffer.Length != 0)
                            {
                                _write(buffer.ToString());

#if NET35
                                buffer = new StringBuilder();
#else
                                buffer.Clear();
#endif
                            }

                            continue;
                        }
                    }
                    else if (state == CliLexerState.ReadingToken)
                    {
                        if (c == '~')
                        {
                            if (buffer.Length == 0)
                            {
                                buffer.Append('~');
                                state = CliLexerState.ReadingText;
                                continue;
                            }
                            else
                            {
                                var token = buffer.ToString();

                                #if NET35
                                buffer = new StringBuilder();
#else
                                buffer.Clear();
#endif

                                if (token == "R")
                                {
                                    Console.ResetColor();
                                }
                                else
                                {
                                    var isBackgroundColor = token[0] == '|';

                                    var color = (ConsoleColor)Enum.Parse(
                                        typeof(ConsoleColor),
                                        isBackgroundColor ? token.Substring(1) : token);

                                    if (isBackgroundColor)
                                    {
                                        Console.BackgroundColor = color;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = color;
                                    }
                                }

                                state = CliLexerState.ReadingText;
                                continue;
                            }
                        }
                    }

                    buffer.Append(c);
                }

                if (state == CliLexerState.ReadingToken)
                {
                    throw new ArgumentException("Invalid token in format string");
                }

                if (newLine)
                {
                    _writeLine(buffer.ToString());
                }
                else
                {
                    _write(buffer.ToString());
                }
            }
        }

        public static string Escape(string value)
        {
            return !string.IsNullOrEmpty(value) ? 
                value
                    .Replace("{", "{{")
                    .Replace("}", "}}")
                    .Replace("~", "~~") : 
                null;
        }
    }
}
