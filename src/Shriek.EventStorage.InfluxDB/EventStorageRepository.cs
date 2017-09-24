using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;

namespace Shriek.EventStorage.InfluxDB
{
    public class EventStorageRepository : IEventStorageRepository
    {
        private readonly InfluxDbContext _dbContext;

        private readonly string _tableName = "events";

        public EventStorageRepository(InfluxDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public void Dispose()
        {
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            var query = $"SELECT * FROM {_tableName} WHERE AggregateId='{aggregateId}' AND Version >= {afterVersion}";
            var result = _dbContext.Client.QueryAsync(query, _dbContext.Options.DatabaseName).Result;

            foreach (var item in result)
                yield return SerieToStoredEvent(item);
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            var query = $"SELECT * FROM {_tableName} WHERE AggregateId = '{aggregateId}' ORDER BY time DESC limit 1";
            var result = _dbContext.Client.QueryAsync(query, _dbContext.Options.DatabaseName)
                    .Result.FirstOrDefault();

            return result == null ? null : SerieToStoredEvent(result);
        }

        public void Store(StoredEvent theEvent)
        {
            var point = new Point()
            {
                Name = _tableName,
                Tags = new Dictionary<string, object>()
                {
                    { "AggregateId", theEvent.AggregateId },
                    { "Version",theEvent.Version }
                },
                Fields = new Dictionary<string, object>()
                {
                    {"Data", theEvent.Data },
                    {"EventType", theEvent.EventType },
                    {"User",theEvent.User },
                    {"CreateDate",theEvent.Timestamp }
                },
                Timestamp = theEvent.Timestamp
            };
            var result = _dbContext.Client.WriteAsync(point, _dbContext.Options.DatabaseName).Result;

            if (!result.Success)
                throw new InfluxDataException("事件插入失败");
        }

        private static StoredEvent SerieToStoredEvent(Serie serie)
        {
            return new StoredEvent
            {
                AggregateId = Guid.Parse(serie.Values[0][serie.Columns.IndexOf("AggregateId")].ToString()),
                Version = int.Parse(serie.Values[0][serie.Columns.IndexOf("Version")].ToString()),
                Data = serie.Values[0][serie.Columns.IndexOf("Data")].ToString().Replace(@"\", ""),
                User = serie.Values[0][serie.Columns.IndexOf("User")].ToString(),
                EventType = serie.Values[0][serie.Columns.IndexOf("EventType")].ToString().Replace(@"\", ""),
                Timestamp = DateTime.Parse(serie.Values[0][0].ToString()),
            };
        }
    }
}