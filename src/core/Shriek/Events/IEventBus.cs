using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : Event;
    }
}