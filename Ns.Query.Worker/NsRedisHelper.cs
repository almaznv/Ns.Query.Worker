using Sider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ns.BpmOnline.Worker
{
    class NsRedisHelper
    {
        public static RedisClient getRedisClient()
        {
            var host = ConfigurationManager.AppSettings["redisHost"];
            var port = ConfigurationManager.AppSettings["redisPort"];
            var settings = RedisSettings.Build()
                      .Host(host)
                      .Port(Convert.ToInt32(port));

            var client = new RedisClient(settings);

            string dbIndex = ConfigurationManager.AppSettings["redisDB"];
            client.Select(Convert.ToInt32(dbIndex));

            return client;
        }
    }
}
