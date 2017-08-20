using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public interface IHandle<TEvent> where TEvent : Event
    {
        void Handle(TEvent e);
    }
}