using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Domains;

namespace Shriek.Storage
{
    public interface IRepository<T> where T : IAggregateRoot, new()
    {
        void Save(AggregateRoot aggregate);

        T GetById(Guid id);
    }
}