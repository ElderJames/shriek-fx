using System;
using Shriek.Events;
using System.Collections.Generic;

namespace Shriek.Domains
{
    public interface IEventProvider<TKey> where TKey : IEquatable<TKey>
    {
        void LoadsFromHistory(IEnumerable<IEvent<TKey>> history);

        IEnumerable<IEvent<TKey>> GetUncommittedChanges();

        void MarkChangesAsCommitted();
    }
}