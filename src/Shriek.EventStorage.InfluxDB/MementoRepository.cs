using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Helpers;
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
            var query = $"SELECT * FROM {_tableName} WHERE AggregateId = '{aggregateId}'";
            var result = _dbContext.Client.QueryAsync(query, _dbContext.Options.DatabaseName)
                .Result.FirstOrDefault();
            if (result == null)
                return null;

            return new Memento()
            {
                aggregateId = Guid.Parse(result.Values[0][result.Columns.IndexOf("AggregateId")].ToString()),
                Version = int.Parse(result.Values[0][result.Columns.IndexOf("Version")].ToString()),
                Data = result.Values[0][result.Columns.IndexOf("Data")].ToString().Replace("\\", ""),
                Timestamp = DateTime.Parse(result.Values[0][result.Columns.IndexOf("time")].ToString())
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
                    {"Data",memento.Data }
                },
                Timestamp = memento.Timestamp
            };

            var result = _dbContext.Client.WriteAsync(point, _dbContext.Options.DatabaseName).Result;

            if (!result.Success)
                throw new InfluxDataException("事件插入失败");
        }
    }
}