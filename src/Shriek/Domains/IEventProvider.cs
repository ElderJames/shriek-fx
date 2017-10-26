using System.Collections.Generic;
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