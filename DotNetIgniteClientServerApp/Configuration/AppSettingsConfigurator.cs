using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace IgnitePersistenceApp.Configuration
{
    public static class AppSettingsConfigurator
    {
        private const string CfgPrefix = "Ignite.";
        private const string CfgHome = "Home";
        private const string CfgJvmOptPrefix = "JvmOption";
        private const string CfgAssemblyPrefix = "Assembly";

        public static IEnumerable<Tuple<string, string>> GetArgs(
            NameValueCollection args)
        {
            return ((IEnumerable<string>)args.AllKeys).Where<string>((Func<string, bool>)(x => x.StartsWith("Ignite.", StringComparison.OrdinalIgnoreCase))).Select<string, Tuple<string, string>>((Func<string, Tuple<string, string>>)(k => Tuple.Create<string, string>(AppSettingsConfigurator.Replace(k), args[k])));
        }

        private static string Replace(string key)
        {
            key = key.Substring("Ignite.".Length);
            key = key.Equals("Home", StringComparison.OrdinalIgnoreCase) ? "IgniteHome" : key;
            key = key.StartsWith("JvmOption", StringComparison.OrdinalIgnoreCase) ? "J" : key;
            key = key.StartsWith("Assembly", StringComparison.OrdinalIgnoreCase) ? "Assembly" : key;
            return key;
        }
    }
}
