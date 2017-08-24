using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shriek.Samples.Model;

namespace Shriek.Samples.Queries
{
    public interface ITodoQuery
    {
        Task<Todo> GetById(Guid aggregateId);

        Task<IEnumerable<Todo>> GetList();
    }
}