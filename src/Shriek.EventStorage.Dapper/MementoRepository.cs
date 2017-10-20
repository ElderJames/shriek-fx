using System;
using System.Data;
using Dapper;
using Shriek.EventSourcing;
using Shriek.IoC;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.Dapper
{
    public class MementoRepository : IMementoRepository
    {
        private readonly IServiceProvider container;

        public MementoRepository(IServiceProvider container)
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

        public Memento GetMemento<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            Memento result = null;
            DapperExecute(conn =>
            {
                result = conn.QueryFirstOrDefault<Memento>($@"SELECT * FROM memento_store WHERE 'AggregateId' = '{aggregateId}' ORDER BY 'Timestamp' DESC");
            });

            return result;
        }

        public void SaveMemento(Memento memento)
        {
            DapperExecute(conn =>
            {
                conn.Execute(
                    $@"INSERT INTO memento_store ('AggregateId','Data','Timestamp','Version') VALUES (@AggregateId,@Data,@Timestamp,@Version)",
                    memento);
            });
        }
    }
}