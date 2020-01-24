using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Common;

namespace IgnitePersistenceApp.Configuration
{
    public static class Configurator
    {
        public const string CmdIgniteHome = "IgniteHome";
        private const string CmdSpringCfgUrl = "SpringConfigUrl";
        private const string CmdJvmDll = "JvmDll";
        private const string CmdJvmClasspath = "JvmClasspath";
        private const string CmdSuppressWarn = "SuppressWarnings";
        public const string CmdJvmOpt = "J";
        public const string CmdAssembly = "Assembly";
        private const string CmdJvmMinMem = "JvmInitialMemoryMB";
        private const string CmdJvmMaxMem = "JvmMaxMemoryMB";
        private const string CmdConfigSection = "ConfigSectionName";
        private const string CmdConfigFile = "ConfigFileName";
        private const string CmdForceTestClasspath = "ForceTestClasspath";

        public static IgniteConfiguration GetConfiguration(Tuple<string, string>[] args)
        {
            List<string> stringList1 = new List<string>();
            List<string> stringList2 = new List<string>();
            IgniteConfiguration cfg = Configurator.ReadConfigurationSection(args) ?? new IgniteConfiguration();
            foreach (Tuple<string, string> tuple in args)
            {
                if (string.IsNullOrWhiteSpace(tuple.Item2))
                    throw new IgniteException(string.Format("Missing argument value: '{0}'. See 'Apache.Ignite.exe /help'", (object)tuple.Item1));
                Tuple<string, string> arg0 = tuple;
                Func<string, bool> func = (Func<string, bool>)(x => arg0.Item1.Equals(x, StringComparison.OrdinalIgnoreCase));
                if (func("IgniteHome"))
                    cfg.IgniteHome = tuple.Item2;
                else if (func("SpringConfigUrl"))
                    cfg.SpringConfigUrl = tuple.Item2;
                else if (func("JvmDll"))
                    cfg.JvmDllPath = tuple.Item2;
                else if (func("JvmClasspath"))
                    cfg.JvmClasspath = tuple.Item2;
                else if (func("SuppressWarnings"))
                    cfg.SuppressWarnings = bool.TrueString.Equals(tuple.Item2, StringComparison.OrdinalIgnoreCase);
                else if (func("JvmInitialMemoryMB"))
                    cfg.JvmInitialMemoryMb = ConfigValueParser.ParseInt(tuple.Item2, "JvmInitialMemoryMB");
                else if (func("JvmMaxMemoryMB"))
                    cfg.JvmMaxMemoryMb = ConfigValueParser.ParseInt(tuple.Item2, "JvmMaxMemoryMB");
                else if (func("J"))
                    stringList1.Add(tuple.Item2);
                else if (func("Assembly"))
                    stringList2.Add(tuple.Item2);
                else if (func("ForceTestClasspath") && tuple.Item2 == "true")
                    Environment.SetEnvironmentVariable("IGNITE_NATIVE_TEST_CLASSPATH", "true");
                else if (!func("ConfigFileName") && !func("ConfigSectionName"))
                    throw new IgniteException(string.Format("Unknown argument: '{0}'. See 'Apache.Ignite.exe /help'", (object)tuple.Item1));
            }
            if (stringList1.Count > 0)
            {
                if (cfg.JvmOptions == null)
                    cfg.JvmOptions = (ICollection<string>)stringList1;
                else
                    stringList1.ForEach((Action<string>)(val => cfg.JvmOptions.Add(val)));
            }
            if (stringList2.Count > 0)
            {
                if (cfg.Assemblies == null)
                    cfg.Assemblies = (ICollection<string>)stringList2;
                else
                    stringList2.ForEach((Action<string>)(val => cfg.Assemblies.Add(val)));
            }
            return cfg;
        }

        private static IgniteConfiguration ReadConfigurationSection(
            Tuple<string, string>[] args)
        {
            string fileName = Configurator.FindValue((IEnumerable<Tuple<string, string>>)args, "ConfigFileName");
            string sectionName = Configurator.FindValue((IEnumerable<Tuple<string, string>>)args, "ConfigSectionName");
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(sectionName))
                return (IgniteConfiguration)null;
            System.Configuration.Configuration configuration = string.IsNullOrEmpty(fileName) ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None) : ConfigurationManager.OpenMappedExeConfiguration(Configurator.GetConfigMap(fileName), ConfigurationUserLevel.None);
            IgniteConfigurationSection configurationSection = string.IsNullOrEmpty(sectionName) ? configuration.Sections.OfType<IgniteConfigurationSection>().FirstOrDefault<IgniteConfigurationSection>() : (IgniteConfigurationSection)configuration.GetSection(sectionName);
            if (configurationSection == null)
                throw new ConfigurationErrorsException(string.Format("Could not find {0} in current application configuration", (object)typeof(IgniteConfigurationSection).Name));
            return configurationSection.IgniteConfiguration;
        }

        private static ExeConfigurationFileMap GetConfigMap(string fileName)
        {
            string fullPath = Path.GetFullPath(fileName);
            if (!File.Exists(fullPath))
                throw new ConfigurationErrorsException("Specified config file does not exist: " + fileName);
            return new ExeConfigurationFileMap()
            {
                ExeConfigFilename = fullPath
            };
        }

        private static string FindValue(IEnumerable<Tuple<string, string>> args, string name)
        {
            return args.Reverse<Tuple<string, string>>().Where<Tuple<string, string>>((Func<Tuple<string, string>, bool>)(x => name.Equals(x.Item1, StringComparison.OrdinalIgnoreCase))).Select<Tuple<string, string>, string>((Func<Tuple<string, string>, string>)(x => x.Item2)).FirstOrDefault<string>();
        }
    }
}
