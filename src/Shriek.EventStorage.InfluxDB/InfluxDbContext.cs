using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.ClientModules;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace Shriek.EventStorage.InfluxDB
{
    public class InfluxDbContext
    {
        public InfluxDbContext(InfluxDBOptions options)
        {
            Options = options;
            var client = new InfluxDbClient(options.Host, options.UserName, options.Password, InfluxDbVersion.v_1_0_0);
            this.Client = client.Client;
            this.Database = client.Database;
            Ensure().Wait();
        }

        public IDatabaseClientModule Database { get; }

        public IBasicClientModule Client { get; }

        public InfluxDBOptions Options { get; }

        public async Task Ensure()
        {
            var dbs = await Database.GetDatabasesAsync();

            if (dbs.All(x => x.Name != Options.DatabaseName))
                await Database.CreateDatabaseAsync(Options.DatabaseName);
        }

        /// <summary>
        /// 查询一个Serie
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Serie> QueryAsync(string query)
        {
            var result = await Client.QueryAsync(query, Options.DatabaseName);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// 写入一个Point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task<IInfluxDataApiResponse> WriteAsync(Point point)
        {
            return await Client.WriteAsync(point, Options.DatabaseName);
        }
    }
}