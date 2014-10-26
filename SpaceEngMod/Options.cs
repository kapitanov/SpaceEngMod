using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceEngMod
{
    public sealed class Options
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        public Options(string name)
        {
            name = name ?? "";
            var body = TryExtractBody(name);
            if (body == null)
            {
                return;
            }

            var parts = body.Split(';');
            foreach (var part in parts)
            {
                var v = part.Split(new[] {'='}, 2);
                if (v.Length == 2)
                {
                    _properties[v[0].ToLowerInvariant()] = v[1];
                }
            }
        }

        public bool Has(string key)
        {
            return _properties.ContainsKey(key.ToLowerInvariant());
        }

        public T Get<T>(string key, T @default)
        {
            try
            {
                string value;
                if (!_properties.TryGetValue(key.ToLowerInvariant(), out value))
                {
                    return @default;
                }

                var t = (T) Convert.ChangeType(value, typeof (T));
                return t;
            }
            catch
            {
                return @default;
            }
        }

        public override string ToString()
        {
            return string.Join("\n", _properties.Select(p => string.Format("{0} = '{1}'", p.Key, p.Value)));
        }

        private static string TryExtractBody(string name)
        {
            var index = 0;
            for (; index < name.Length && name[index] != '{'; index++)
            { }

            if (index >= name.Length)
            {
                return null;
            }

            var from = index;

            for (; index < name.Length && name[index] != '}'; index++)
            { }

            if (index >= name.Length)
            {
                return null;
            }

            var to = index;

            var body = name.Substring(from + 1, to - from - 1);
            return body;
        }
    }
}