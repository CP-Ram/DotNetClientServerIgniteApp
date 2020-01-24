using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Eviction;
using Apache.Ignite.Core.Cluster;
using Apache.Ignite.Core.Communication.Tcp;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using DotNetIgniteClientServerApp.CacheStoreFactory;
using DotNetIgniteClientServerApp.Interface;
using DotNetIgniteClientServerApp.Models;
using IgnitePersistenceApp.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetIgniteClientServerApp
{
    class Program
    {
        private static IIgnite IGNITE_SERVER;
        private static IIgnite IGNITE_CLIENT_FIRST;
        private static IIgnite IGNITE_CLIENT_SECOND;


        private static string _CACHE_NAME1 = "TEST_CACHE_01_01_01";
        private static string _CACHE_NAME2 = "TEST_CACHE_02_02_02";

        private static string Node_FIRST_NAME = "_Node_0";
        private static string Node_SECOND_NAME = "_Node_1";

        private static async Task StartServerAsync(string[] args) //string[] args
        {
            try
            {
                if (args != null && args.Length > 0 && IGNITE_SERVER == null)
                {
                    Tuple<string, string>[] array = AppSettingsConfigurator.GetArgs(ConfigurationManager.AppSettings).Concat<Tuple<string, string>>(ArgsConfigurator.GetArgs((IEnumerable<string>)args)).ToArray<Tuple<string, string>>();

                    // Connect to the cluster.
                    IGNITE_SERVER = Ignition.Start(Configurator.GetConfiguration(array)); //configuration //"server-config.xml" //Configurator.GetConfiguration(array)
                    // Activate the cluster.
                    // This is required only if the cluster is still inactive.
                    IGNITE_SERVER.GetCluster().SetActive(true);

                    IEnumerable<IClusterNode> nodes = IGNITE_SERVER.GetCluster().ForServers().GetNodes();
                    IGNITE_SERVER.GetCluster().SetBaselineTopology(nodes);
                }
                else
                {
                    new Exception("Application Parameters can not be null/Empty!!!");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task StartClientAsync(string nodeName)
        {
            try
            {
                if (IGNITE_CLIENT_FIRST == null)
                {
                    Ignition.ClientMode = true;
                    // Connect to the cluster.
                    //IGNITE_CLIENT_FIRST = Ignition.Start("client-config.xml");
                    IGNITE_CLIENT_FIRST = Ignition.Start(GetIgniteConfiguration(nodeName));
                }
                else if (IGNITE_CLIENT_SECOND == null)
                {
                    Ignition.ClientMode = true;
                    // Connect to the cluster.
                    //IGNITE_CLIENT_SECOND = Ignition.Start("client-config.xml");
                    IGNITE_CLIENT_SECOND = Ignition.Start(GetIgniteConfiguration(nodeName));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static IgniteConfiguration GetIgniteConfiguration(string nodeName)
        {
            IgniteConfiguration config = null;
            try
            {
                config = new IgniteConfiguration
                {

                    IgniteInstanceName = nodeName,

                    DiscoverySpi = new TcpDiscoverySpi
                    {
                        IpFinder = new TcpDiscoveryStaticIpFinder
                        {
                            Endpoints = new[] { "127.0.0.1" }//127.0.0.1 or ip
                        },
                        SocketTimeout = TimeSpan.FromSeconds(3000)
                    },
                    DataStorageConfiguration = new DataStorageConfiguration()
                    {
                        DefaultDataRegionConfiguration = new DataRegionConfiguration()
                        {
                            Name = "IgniteDataRegion",
                            PersistenceEnabled = true
                        },
                        StoragePath = "C:\\client\\storage",
                        WalPath = "C:\\client\\wal",
                        WalArchivePath = "C:\\client\\walArchive"
                    },
                    WorkDirectory = "C:\\client\\work",
                    // Explicitly configure TCP communication SPI by changing local port number for
                    // the nodes from the first cluster.
                    CommunicationSpi = new TcpCommunicationSpi()
                    {
                        LocalPort = 47100
                    },
                    PeerAssemblyLoadingMode = Apache.Ignite.Core.Deployment.PeerAssemblyLoadingMode.CurrentAppDomain

                };
            }
            catch (Exception ex)
            {
                throw;
            }
            return config;
        }

        private static async Task<CacheConfiguration> CreateCacheConfigurationTemplate(string cacheName)
        {
            var queryEntity = new QueryEntity(typeof(string), typeof(Student));
            var queryList = new List<QueryEntity> { queryEntity };

            var templateName = cacheName.Remove(cacheName.Length - 1, 1) + "*";

            var cacheCFG = new CacheConfiguration(templateName)
            {
                Name = cacheName,
                CacheStoreFactory = new DataCacheStoreFactory(),
                KeepBinaryInStore = false,  // Cache store works with deserialized data.
                ReadThrough = true,
                WriteThrough = true,
                QueryEntities = queryList,
                DataRegionName = "IgniteDataRegion",
                EvictionPolicy = new LruEvictionPolicy
                {
                    MaxSize = 1000000
                }
            };

            return cacheCFG;
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Ignite Server
            StartServerAsync(args);
            
            //Ignite Client
            StartClientAsync(Node_FIRST_NAME);
            StartClientAsync(Node_SECOND_NAME);


            //Create and configure cache configuration on node1
            //var cacheCfg = CreateCacheConfigurationTemplate(_CACHE_NAME1).GetAwaiter().GetResult();
            //IGNITE_CLIENT_FIRST.AddCacheConfiguration(cacheCfg);
            //IGNITE_CLIENT_FIRST.GetOrCreateCache<string, ICustomCacheStore>(_CACHE_NAME2);
            IGNITE_CLIENT_FIRST.GetOrCreateCache<string, ICustomCacheStore>(_CACHE_NAME2);
            //issue while getting cache configuration from Ignite client node,if connfiguration is not get added.
            OperationOnCache().GetAwaiter().GetResult();

            Thread.Sleep(10000);
        }

      

        public static async Task OperationOnCache()
        {
            var repeatProcess = "n";
            do
            {
                try
                {
               
                    var cacheConfig = IGNITE_CLIENT_SECOND.GetCache<string, ICustomCacheStore>(_CACHE_NAME2).GetConfiguration();
                    if (cacheConfig == null)
                    {

                        IGNITE_CLIENT_SECOND.AddCacheConfiguration(await CreateCacheConfigurationTemplate(_CACHE_NAME2));
                        //next to do operation on cache
                        //get or create cache
                        //query on cache
                    }
                    else
                    {
                        //get or create cache
                        //query on cache
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.ToString());
                }

            Console.WriteLine("Do you want to repeat process.(y/n)");
            repeatProcess = Console.ReadLine();
            } while (repeatProcess == "y" || repeatProcess == "Y" || repeatProcess == "");
        }
    }
}
