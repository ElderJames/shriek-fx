using System;
using Shriek.Events;

namespace Shriek.Samples.Events
{
    public class TodoDeletedEvent : Event<Guid>
    {
    }
}