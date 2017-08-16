using Shriek.Domains;
using System.Runtime.InteropServices.ComTypes;
using Shriek.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Commands
{
    public class CommandContext : ICommandContext
    {
        private IServiceProvider Container;

        public CommandContext(IServiceProvider Container)
        {
            this.Container = Container;
        }

        public IDictionary<string, object> Items => new Dictionary<string, object>();

        TAggregateRoot ICommandContext.GetAggregateRoot<TAggregateRoot>(Guid key, Func<TAggregateRoot> initFromRepository)
        {
            var repository = Container.GetService(typeof(IRepository<TAggregateRoot>)) as IRepository<TAggregateRoot>;

            if (repository != null)
            {
                var root = repository.GetById(key);
                if (root != null)
                    return root;
            }

            return initFromRepository();
        }
    }
}