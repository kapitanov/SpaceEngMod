using System;
using System.IO;
using System.Linq;
using Sandbox.ModAPI;
using VRageMath;

namespace SpaceEngMod
{
    public static class Log
    {
        private static readonly TextWriter FailureTextWriter = TextWriter.Null;
        private static TextWriter _writer;
        private static int _indentLevel;

        private static TextWriter GetWriter()
        {
            try
            {
                if (_writer == null)
                {
                    _writer = MyAPIGateway.Utilities.WriteFileInGlobalStorage(string.Format("SpaceEngMod.{0}.log", DateTime.Now.Ticks));
                    Write("=== Log Started ===");
                }

                return _writer;
            }
            catch (Exception e)
            {
                if (MyAPIGateway.Utilities != null)
                {
                    MyAPIGateway.Utilities.ShowNotification("Unable to write into log file due to '" + e.Message + "'");
                }
                
                return FailureTextWriter;
            }
        }

        public static void Release()
        {
        }

        public static void Write(string message)
        {
            var writer = GetWriter();

            writer.Write("[");
            writer.Write(DateTime.Now.ToString("d.MM.yyyy HH:mm:ss"));
            writer.Write("] ");
            for (var i = 0; i < _indentLevel; i++)
            {
                writer.Write('\t');
            }
            writer.Write(message);
            writer.WriteLine();

            writer.Flush();
        }

        public static void Write(string message, params object[] args)
        {
            Write(FormatMessage(message, args));
        }

        public static void In(string message)
        {
            _indentLevel++;
            Write(message + " IN");
        }

        public static void In(string message, params object[] args)
        {
            In(FormatMessage(message, args));
        }

        public static void Out(string message)
        {
            Write(message + " OUT");
            _indentLevel--;
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

        private static string FormatMessage(string message, object[] args)
        {
            return string.Format(message, args.Select(FormatArgument).Cast<object>().ToArray());
        }

        private static string FormatArgument(object arg)
        {
            if (arg is Vector3I)
            {
                var value = (Vector3I) arg;
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