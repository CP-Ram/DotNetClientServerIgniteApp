using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IgnitePersistenceApp.Configuration
{
    public static class ArgsConfigurator
    {
        public static IEnumerable<Tuple<string, string>> GetArgs(
            IEnumerable<string> args)
        {
            return args.Select<string, string>((Func<string, string>)(x => x.Trim().TrimStart('-'))).Select<string, string[]>((Func<string, string[]>)(x =>
            {
                if (!x.StartsWith("J-"))
                    return x.Split(new char[1] { '=' }, 2);
                return new string[2] { "J", x.Substring("J".Length) };
            })).Select<string[], Tuple<string, string>>((Func<string[], Tuple<string, string>>)(x => Tuple.Create<string, string>(x[0], x.Length > 1 ? x[1] : string.Empty)));
        }
    }
}
