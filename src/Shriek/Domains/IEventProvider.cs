using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;

namespace Shriek.Domains
{
    public interface IEventProvider
    {
        void LoadsFromHistory(IEnumerable<Event> history);

        IEnumerable<Event> GetUncommittedChanges();

        void MarkChangesAsCommitted();
    }
}