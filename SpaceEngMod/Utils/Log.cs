using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

using VRageMath;

namespace SPX.Station.Infrastructure.Utils
{
    public static class Log
    {
        private interface ILogWriter : IDisposable
        {
            ILogWriter InheritWriter();
            void Write(string line);
        }

        private sealed class FileLogWriter : ILogWriter
        {
            private readonly TextWriter _writer;

            public FileLogWriter(TextWriter writer)
            {
                _writer = writer;
            }

            public FileLogWriter()
                : this(
                    MyAPIGateway.Utilities.WriteFileInGlobalStorage(
                        string.Format("SPX.SpacePort.Mod.{0}.log",
                        DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss"))))
            { }

            ILogWriter ILogWriter.InheritWriter()
            {
                return this;
            }

            void ILogWriter.Write(string line)
            {
                _writer.Write(line);
                _writer.Flush();
            }

            public void Dispose()
            {
                _writer.Dispose();
            }
        }

        private sealed class PendingLogWriter : ILogWriter
        {
            private readonly List<string> _pendingMessages = new List<string>();

            ILogWriter ILogWriter.InheritWriter()
            {
                if (MyAPIGateway.Utilities == null)
                {
                    return this;
                }

                ILogWriter writer = new FileLogWriter();
                foreach (var message in _pendingMessages)
                {
                    writer.Write(message);
                }
                _pendingMessages.Clear();

                return writer;
            }

            void ILogWriter.Write(string line)
            {
                _pendingMessages.Add(line);
            }

            public void Dispose() { }
        }

        private static ILogWriter _writer = new PendingLogWriter();

        private static ILogWriter GetWriter()
        {
            try
            {
                if (_writer == null)
                {
                    _writer = new PendingLogWriter();
                }

                var inheritedWriter = _writer.InheritWriter();
                if (inheritedWriter != null)
                {
                    _writer = inheritedWriter;
                }
            }
            catch (Exception e)
            {
                if (MyAPIGateway.Utilities != null)
                {
                    MyAPIGateway.Utilities.ShowNotification("Unable to write into log file due to '" + e.Message + "'");
                }

                _writer = new PendingLogWriter();
            }

            return _writer;
        }

        public static void Initialize()
        {
            GetWriter();
        }

        public static void Release()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }

        public static void Write(string message)
        {
            var writer = GetWriter();

            var messageBuilder = new StringBuilder();
            messageBuilder.Append("[");
            messageBuilder.Append(DateTime.Now.ToString("d.MM.yyyy HH:mm:ss"));
            messageBuilder.Append("] ");

            messageBuilder.Append(message);
            messageBuilder.Append('\n');

            writer.Write(messageBuilder.ToString());
        }

        public static void Write(string message, params object[] args)
        {
            Write(FormatMessage(message, args));
        }

        public static void In(string message)
        {
            Write(message + " IN");
        }

        public static void In(string message, params object[] args)
        {
            In(FormatMessage(message, args));
        }

        public static void Out(string message)
        {
            Write(message + " OUT");
        }

        public static void Out(string message, params object[] args)
        {
            Out(FormatMessage(message, args));
        }

        public static LogScopeToken Scope(string name)
        {
            return new LogScopeToken(name);
        }

        public static LogScopeToken Scope(string name, params object[] args)
        {
            return Scope(FormatMessage(name, args));
        }

        public struct LogScopeToken : IDisposable
        {
            private readonly string name;

            public LogScopeToken(string name) : this()
            {
                this.name = name;
                In(name);
            }

            public void Dispose()
            {
                Out(name);
            }
        }

        public static void Error(Exception e)
        {
            Write("ERROR\n" + e);
        }

        public static void Error(Exception e, string where)
        {
            Write("ERROR IN {" + where + "}\n" + e);
        }

        private static string FormatMessage(string message, object[] args)
        {
            return string.Format(message, args.Select(FormatArgument).Cast<object>().ToArray());
        }

        private static string FormatArgument(object arg)
        {
            if (arg is Vector3I)
            {
                var value = (Vector3I)arg;
                return string.Format("{{{0}, {1}, {2}}}", value.X, value.Y, value.Z);
            }

            if (arg is float)
            {
                return ((float)arg).ToString();
            }

            if (arg is int)
            {
                return ((int)arg).ToString();
            }

            if (arg is bool)
            {
                return (bool)arg ? "YES" : "NO";
            }

            if (arg == null)
            {
                return "null";
            }

            return "'" + arg + "'";
        }
    }
}