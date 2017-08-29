using Microsoft.EntityFrameworkCore;
using Shriek.Samples.Aggregates;
using System;
using System.Linq;

namespace Shriek.Samples.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private TodoDbContext context;

        public TodoRepository(TodoDbContext context)
        {
            this.context = context;
        }

        public TodoAggregateRoot Rebuild(Guid aggregateId)
        {
            var root = this.context.Set<TodoAggregateRoot>().FirstOrDefault(x => x.AggregateId == aggregateId);
            context.Attach(root);
            return root;
        }

        public bool Change(TodoAggregateRoot root)
        {
            var _root = this.context.Set<TodoAggregateRoot>().Update(root);
            return _root.State == EntityState.Modified;
        }

        public bool Create(TodoAggregateRoot root)
        {
            var _root = context.Set<TodoAggregateRoot>().Add(root);
            return _root.State == EntityState.Added;
        }

        public bool Delete(TodoAggregateRoot root)
        {
            var _root = context.Set<TodoAggregateRoot>().Remove(root);
            return _root.State == EntityState.Deleted;
        }

        public int Save()
        {
            return context.SaveChanges();
        }
    }
}