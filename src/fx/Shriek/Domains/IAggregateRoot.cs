using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Domains
{
    public interface IAggregateRoot
    {
        Guid AggregateId { get; }

        int Version { get; }
    }
}