using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.InfluxDB
{
    public class MementoRepository : IMementoRepository
    {
        private readonly InfluxDbContext _dbContext;

        private readonly string _tableName = "memento";

        public MementoRepository(InfluxDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public Memento GetMemento(Guid aggregateId)
        {
            var result = _dbContext.Client.QueryAsync(_dbContext.Options.DatabaseName,
                    $"SELECT * FROM {_tableName} WHERE AggregateId = {aggregateId};")
                .Result.FirstOrDefault();

            return new Memento()
            {
                aggregateId = Guid.Parse(result.Tags["AggregateId"]),
                Version = int.Parse(result.Tags["Version"]),
                Data = result.Values[result.Columns.IndexOf("Data")].ToString(),
                Timestamp = DateTime.Parse(result.Values[result.Columns.IndexOf("CreateDate")].ToString()),
            };
        }

        public void SaveMemento(Memento memento)
        {
            var point = new Point()
            {
                Name = _tableName,
                Tags = new Dictionary<string, object>()
                {
                    {"AggregateId",memento.aggregateId },
                    {"Version",memento.Version }
                },
                Fields = new Dictionary<string, object>()
                {
                    {"Data",memento.Data },
                    {"CreateDate",memento.Timestamp }
                },
                Timestamp = memento.Timestamp
            };

            var result = _dbContext.Client.WriteAsync(point).Result;

            if (!result.Success)
                throw new InfluxDataException("事件插入失败");
        }
    }
}