using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.EventStorage.InfluxDB
{
    public class EventStorageRepository : IEventStorageRepository
    {
        private readonly InfluxDbContext dbContext;

        private const string TableName = "events";

        public EventStorageRepository(InfluxDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Dispose()
        {
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            var query = $"SELECT * FROM {TableName} WHERE AggregateId='{aggregateId}' AND Version >= {afterVersion}";
            var result = dbContext.QueryAsync(query).Result;

            return result == null ? new StoredEvent[] { } : SerieToStoredEvent(result);
        }

        public StoredEvent GetLastEvent<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            var query = $"SELECT * FROM {TableName} WHERE AggregateId = '{aggregateId}' ORDER BY time DESC LIMIT 1";
            var result = dbContext.QueryAsync(query).Result;

            return result == null ? null : SerieToStoredEvent(result).FirstOrDefault();
        }

        public void Store(StoredEvent theEvent)
        {
            var point = new Point()
            {
                Name = TableName,
                Tags = new Dictionary<string, object>()
            {
                {"AggregateId", theEvent.AggregateId}
            },
                Fields = new Dictionary<string, object>()
            {
                {"Data", theEvent.Data},
                {"MessageType", theEvent.MessageType},
                {"User", theEvent.User},
                {"Version", theEvent.Version}
            },
                Timestamp = theEvent.Timestamp
            };

            var result = dbContext.WriteAsync(point).Result;

            if (!result.Success)
                throw new InfluxDataException("事件插入失败");
        }

        private static IEnumerable<StoredEvent> SerieToStoredEvent(Serie serie)
        {
            return serie.Values.Select(item => new StoredEvent
            {
                AggregateId = item[serie.Columns.IndexOf("AggregateId")].ToString(),
                Version = int.Parse(item[serie.Columns.IndexOf("Version")].ToString()),
                Data = item[serie.Columns.IndexOf("Data")].ToString().Replace(@"\", string.Empty),
                User = item[serie.Columns.IndexOf("User")].ToString(),
                MessageType = item[serie.Columns.IndexOf("MessageType")].ToString().Replace(@"\", string.Empty),
                Timestamp = DateTime.Parse(item[0].ToString())
            });
        }
    }
}