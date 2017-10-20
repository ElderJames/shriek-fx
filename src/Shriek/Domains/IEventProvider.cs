using System;
using Shriek.Events;
using System.Collections.Generic;

namespace Shriek.Domains
{
    public interface IEventProvider
    {
        void LoadsFromHistory(IEnumerable<Event> history);

        IEnumerable<Event> GetUncommittedChanges();

        void MarkChangesAsCommitted();
    }
}