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

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey eventId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            var query = $"SELECT * FROM {TableName} WHERE EventId = '{eventId}' AND Version >= {afterVersion}";
            var result = dbContext.QueryAsync(query).Result;

            return result == null ? new StoredEvent[] { } : SerieToStoredEvent(result);
        }

        public StoredEvent GetLastEvent<TKey>(TKey eventId)
            where TKey : IEquatable<TKey>
        {
            var query = $"SELECT * FROM {TableName} WHERE EventId = '{eventId}' ORDER BY time DESC LIMIT 1";
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
                {"AggregateId", theEvent.EventId}
            },
                Fields = new Dictionary<string, object>()
            {
                {"Data", theEvent.Data},
                {"EventType", theEvent.EventType},
                {"Creator", theEvent.Creator},
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
            (
                item[serie.Columns.IndexOf("EventId")].ToString(),
                item[serie.Columns.IndexOf("EventType")].ToString().Replace(@"\", string.Empty),
                item[serie.Columns.IndexOf("Data")].ToString().Replace(@"\", string.Empty),
                int.Parse(item[serie.Columns.IndexOf("Version")].ToString()),
                item[serie.Columns.IndexOf("Creator")].ToString()
            )
            {
                Timestamp = DateTime.Parse(item[0].ToString()),
            });
        }
    }
}