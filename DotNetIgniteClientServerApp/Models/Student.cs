using Apache.Ignite.Core.Cache.Configuration;
using DotNetIgniteClientServerApp.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetIgniteClientServerApp.Models
{
    public class Student: ICustomCacheStore
    {
        [QuerySqlField]
        public string Name { get; set; }

        [QuerySqlField]
        public string ID { get; set; }
    }
}
