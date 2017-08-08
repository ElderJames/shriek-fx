using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public interface IHandle<TEvent, TAggregateId> where TEvent : Event<TAggregateId>
    {
        void Handle(TEvent e);
    }
}