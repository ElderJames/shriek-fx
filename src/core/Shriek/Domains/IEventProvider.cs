using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;

namespace Shriek.Domains
{
    public interface IEventProvider<TAggregateId>
    {
        void LoadsFromHistory(IEnumerable<Event<TAggregateId>> history);

        IEnumerable<Event<TAggregateId>> GetUncommittedChanges();
    }
}