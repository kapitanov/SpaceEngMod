using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceEngMod
{
    public sealed class CommandArgs
    {
        private readonly string[] _args;

        private CommandArgs(string[] args)
        {
            _args = args;
        }

        public string GetString(int index)
        {
            return (_args.Length > index) ? _args[index] : null;
        }

        public int? GetInteger(int index)
        {
            return (_args.Length > index) ? (int?)int.Parse(_args[index]) : null;
        }

        public float? GetFloat(int index)
        {
            return (_args.Length > index) ? (float?)float.Parse(_args[index]) : null;
        }

        public override string ToString()
        {
            return string.Join(" ", _args.Select(arg => "\"" + arg + "\""));
        }

        private enum ParserState { NotInQuotedText, InQuotedText }

        public static CommandArgs Parse(IEnumerable<string> args)
        {
            return Parse(string.Concat(args));
        }

        public static CommandArgs Parse(string text)
        {
            var args = new List<string>();

            var arg = new StringBuilder();
            var state = ParserState.NotInQuotedText;

            foreach (var ch in text)
            {
                if (state == ParserState.NotInQuotedText)
                {
                    if (ch == ' ')
                    {
                        if (arg.Length > 0)
                        {
                            args.Add(arg.ToString());
                            arg.Clear();
                        }
                        continue;
                    }

                    if (ch == '"')
                    {
                        if (arg.Length > 0)
                        {
                            args.Add(arg.ToString());
                            arg.Clear();
                        }
                        state = ParserState.InQuotedText;
                        continue;
                    }

                    arg.Append(ch);
                }
                else
                {
                    if (ch == '"')
                    {
                        args.Add(arg.ToString());
                        arg.Clear();
                        state = ParserState.NotInQuotedText;
                        continue;
                    }

                    arg.Append(ch);
                }
            }

            if (arg.Length > 0)
            {
                args.Add(arg.ToString());
                arg.Clear();
            }

            return new CommandArgs(args.ToArray());
        }
    }
}