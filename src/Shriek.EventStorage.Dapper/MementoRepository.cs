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
            using (var conn = options.DbConnection)
            {
                conn.Open();
                sqlAction(conn);
                conn.Close();
            }
        }

        public Memento GetMemento(Guid aggregateId)
        {
            Memento result = null;
            DapperExecute(conn =>
            {
                result = conn.QueryFirstOrDefault<Memento>($"SELECT * FROM memento_store WHERE AggregateId = '{aggregateId}' ORDER BY Timestamp DESC");
            });

            return result;
        }

        public void SaveMemento(Memento memento)
        {
            DapperExecute(conn =>
            {
                conn.Execute(
                    $@"INSERT INTO memento_store (AggregateId,Data,Timestamp,Version) VLAUES (@AggregateId,@Data,@Timestamp,@Version)",
                    memento);
            });
        }
    }
}