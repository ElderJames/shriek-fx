using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    internal interface IEvent<TAggregateId>
    {
        TAggregateId Id { get; }
    }
}