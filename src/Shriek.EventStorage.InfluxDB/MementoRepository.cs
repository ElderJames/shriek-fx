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

        private const string TableName = "memento";

        public MementoRepository(InfluxDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public Memento GetMemento(Guid aggregateId)
        {
            var query = $"SELECT * FROM {TableName} WHERE AggregateId = '{aggregateId}' ORDER BY time DESC LIMIT 1";
            var result = _dbContext.QueryAsync(query)
                .Result.FirstOrDefault();

            return result == null ? null : new Memento()
            {
                aggregateId = Guid.Parse(result.Values[0][result.Columns.IndexOf("AggregateId")].ToString()),
                Version = int.Parse(result.Values[0][result.Columns.IndexOf("Version")].ToString()),
                Data = result.Values[0][result.Columns.IndexOf("Data")].ToString().Replace(@"\", string.Empty),
                Timestamp = DateTime.Parse(result.Values[0][0].ToString())
            };
        }

        public void SaveMemento(Memento memento)
        {
            var point = new Point()
            {
                Name = TableName,
                Tags = new Dictionary<string, object>()
                {
                    {"AggregateId",memento.aggregateId }
                },
                Fields = new Dictionary<string, object>()
                {
                    {"Data",memento.Data },
                    {"Version",memento.Version }
                },
                Timestamp = memento.Timestamp
            };

            var result = _dbContext.WriteAsync(point).Result;

            if (!result.Success)
                throw new InfluxDataException("事件插入失败");
        }
    }
}