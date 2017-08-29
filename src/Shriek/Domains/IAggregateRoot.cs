using System;

namespace Shriek.Domains
{
    public interface IAggregateRoot
    {
        Guid AggregateId { get; }

        int Version { get; }

        bool CanCommit { get; }
    }
}