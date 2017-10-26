using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shriek.Samples.Models;

namespace Shriek.Samples.Queries
{
    public interface ITodoQuery
    {
        Task<Todo> GetById(Guid aggregateId);

        Task<IEnumerable<Todo>> GetList();
    }
}