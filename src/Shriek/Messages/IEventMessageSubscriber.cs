using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;

namespace Shriek.Messages
{
    internal interface IEventMessageSubscriber : IMessageSubscriber<Event>
    {
    }
}