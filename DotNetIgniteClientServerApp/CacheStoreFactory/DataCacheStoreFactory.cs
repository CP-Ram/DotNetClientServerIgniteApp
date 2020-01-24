using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using DotNetIgniteClientServerApp.CacheStore;
using DotNetIgniteClientServerApp.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetIgniteClientServerApp.CacheStoreFactory
{
    public class DataCacheStoreFactory : IFactory<ICacheStore<string, ICustomCacheStore>>
    {

        public DataCacheStoreFactory()
        {
        }
        ICacheStore<string, ICustomCacheStore> IFactory<ICacheStore<string, ICustomCacheStore>>.CreateInstance()
        {
            return new DataCacheStore();
        }
    }
}
