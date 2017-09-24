using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.ClientModules;

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
    }
}