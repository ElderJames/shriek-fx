using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shriek.Samples.Models;

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