using Apache.Ignite.Core.Cache.Store;
using DotNetIgniteClientServerApp.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetIgniteClientServerApp.CacheStore
{
    public class DataCacheStore : ICacheStore<string, ICustomCacheStore>
    {
        public void Delete(string key)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public ICustomCacheStore Load(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, ICustomCacheStore>> LoadAll(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public void LoadCache(Action<string, ICustomCacheStore> act, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }

        public void Write(string key, ICustomCacheStore val)
        {
            throw new NotImplementedException();
        }

        public void WriteAll(IEnumerable<KeyValuePair<string, ICustomCacheStore>> entries)
        {
            throw new NotImplementedException();
        }
    }
}
