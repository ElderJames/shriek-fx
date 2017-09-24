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
            var result = _dbContext.Client.QueryAsync(_dbContext.Options.DatabaseName,
                $"SELECT * FROM {_tableName} WHERE AggregateId={aggregateId} AND Version>={afterVersion};").Result;

            foreach (var item in result)
                yield return SerieToStoredEvent(item);
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            var result = _dbContext.Client.QueryAsync(_dbContext.Options.DatabaseName,
                    $"SELECT * FROM {_tableName} WHERE AggregateId = {aggregateId};")
                    .Result.FirstOrDefault();

            return SerieToStoredEvent(result);
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
            var result = _dbContext.Client.WriteAsync(point).Result;

            if (!result.Success)
                throw new InfluxDataException("事件插入失败");
        }

        private StoredEvent SerieToStoredEvent(Serie serie)
        {
            return new StoredEvent
            {
                AggregateId = Guid.Parse(serie.Tags["AggregateId"]),
                Version = int.Parse(serie.Tags["Version"]),
                Data = serie.Values[serie.Columns.IndexOf("Data")].ToString(),
                User = serie.Values[serie.Columns.IndexOf("User")].ToString(),
                EventType = serie.Values[serie.Columns.IndexOf("EventType")].ToString(),
                Timestamp = DateTime.Parse(serie.Values[serie.Columns.IndexOf("CreateDate")].ToString()),
            };
        }
    }
}