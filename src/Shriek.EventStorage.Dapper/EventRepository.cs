using Dapper;
using Shriek.EventSourcing;
using Shriek.IoC;
using Shriek.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Shriek.EventStorage.Dapper
{
    public class EventRepository : IEventStorageRepository
    {
        private readonly IServiceProvider container;

        public EventRepository(IServiceProvider container)
        {
            this.container = container;
        }

        private void DapperExecute(Action<IDbConnection> sqlAction)
        {
            var options = container.GetService<DapperOptions>();
            var conn = options.DbConnection;

            try
            {
                conn.Open();
                sqlAction(conn);
            }
            finally
            {
                conn.Close();
            }
        }

        public void Dispose()
        {
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            var result = Enumerable.Empty<StoredEvent>();
            DapperExecute(conn =>
            {
                result = conn.Query<StoredEvent>($"SELECT * FROM event_store WHERE 'AggregateId' = '{aggregateId}' AND 'Version' >={afterVersion}");
            });

            return result;
        }

        public StoredEvent GetLastEvent<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            StoredEvent result = null;
            DapperExecute(conn =>
            {
                result = conn.QueryFirstOrDefault<StoredEvent>($"SELECT * FROM event_store WHERE 'AggregateId' = '{aggregateId}' ORDER BY 'Timestamp' DESC");
            });

            return result;
        }

        public void Store(StoredEvent theEvent)
        {
            DapperExecute(conn =>
            {
                conn.Execute(
                    $@"INSERT INTO event_store ('AggregateId','Data','MessageType','Timestamp','Version','User') VALUES (@AggregateId,@Data,@MessageType,@Timestamp,@Version,@User)",
                    theEvent);
            });
        }
    }
}