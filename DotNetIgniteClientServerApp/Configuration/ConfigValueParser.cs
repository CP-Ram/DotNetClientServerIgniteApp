using Apache.Ignite.Core;
using Apache.Ignite.Core.Cluster;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace IgnitePersistenceApp.Configuration
{
    public class ConfigValueParser
    {

        public static int ParseInt(string value, string propertyName)
        {
            int result;
            if (int.TryParse(value, out result))
                return result;
            throw new InvalidOperationException(string.Format("Failed to configure Ignite: property '{0}' has value '{1}', which is not an integer.", (object)propertyName, (object)value));
        }
        
    }
}
