using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Extensions.Dapper
{
    internal class DbAction<T> where T : class
    {
        public DbActionTypes ActionType { get; }

        public T Entity { get; }

        public DbAction(DbActionTypes actionType, T entity)
        {
            ActionType = actionType;
            Entity = entity;
        }
    }
}