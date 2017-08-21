using System.Diagnostics;
using System.Threading;
using Shriek.Console.Events;
using Shriek.Events;

namespace Shriek.Console.Handlers
{
    public class ConfigItemEventHandler : IEventHandler<ConfigItemCreatedEvent>, IEventHandler<ConfigItemChangedEvent>
    {
        public void Handle(ConfigItemCreatedEvent e)
        {
            System.Console.WriteLine("here is " + e.Name);
            Thread.Sleep(5000);
            System.Console.WriteLine(e.Name + " finished!");
        }

        public void Handle(ConfigItemChangedEvent e)
        {
            System.Console.WriteLine("here is " + e.Name);
            Thread.Sleep(5000);
            System.Console.WriteLine(e.Name + " finished!");
        }
    }
}