using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.ClientModules;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace Shriek.EventStorage.InfluxDB
{
    public class InfluxDbContext
    {
        public InfluxDbContext(InfluxDBOptions options)
        {
            Options = options;
            var _client = new InfluxDbClient(options.Host, options.UserName, options.Password, InfluxDbVersion.v_1_0_0);
            this.Client = _client.Client;
            this.Database = _client.Database;
            Ensure().Wait();
        }

        public IDatabaseClientModule Database { get; }

        public IBasicClientModule Client { get; }

        public InfluxDBOptions Options { get; }

        public async Task Ensure()
        {
            var dbs = await Database.GetDatabasesAsync();

            if (dbs.All(x => x.Name != Options.DatabaseName))
            {
                await Database.CreateDatabaseAsync(Options.DatabaseName);
            }
        }

        public async Task<IEnumerable<Serie>> QueryAsync(string query)
        {
            return await Client.QueryAsync(query, Options.DatabaseName);
        }

        public async Task<IInfluxDataApiResponse> WriteAsync(Point point)
        {
            return await Client.WriteAsync(point, Options.DatabaseName);
        }
    }
}