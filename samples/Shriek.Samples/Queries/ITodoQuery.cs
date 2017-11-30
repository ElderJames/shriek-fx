using Shriek.Samples.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shriek.Samples.Queries
{
    public interface ITodoQuery
    {
        Task<Todo> GetById(Guid aggregateId);

        Task<IEnumerable<Todo>> GetList();
    }
}