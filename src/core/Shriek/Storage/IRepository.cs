using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Domains;

namespace Shriek.Storage
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate, int expectedVersion);

        T GetById(Guid id);
    }
}