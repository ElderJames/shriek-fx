using System.Linq;
using System;
using System.Collections.Generic;
using Shriek.Samples.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Shriek.Samples.Queries
{
    public class TodoQuery : ITodoQuery
    {
        private TodoDbContext context;

        public TodoQuery(TodoDbContext context)
        {
            this.context = context;
        }

        public async Task<Todo> GetById(Guid aggregateId)
        {
            return await context.Set<Todo>().FirstOrDefaultAsync(x => x.AggregateId == aggregateId);
        }

        public async Task<IEnumerable<Todo>> GetList()
        {
            return await context.Set<Todo>().ToListAsync();
        }
    }
}