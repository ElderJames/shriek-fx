using System.Threading;
using Shriek.Events;
using Shriek.Samples.Events;

namespace Shriek.Sample.EventStorage.Mongo.Handlers
{
    public class TodoEventHandler : IEventHandler<TodoCreatedEvent>, IEventHandler<TodoChangedEvent>
    {
        public void Handle(TodoCreatedEvent e)
        {
            System.Console.WriteLine($"here is {nameof(TodoCreatedEvent)}:" + e.Name);
            Thread.Sleep(5000);
            System.Console.WriteLine($"{e.Name} finished!");
        }

        public void Handle(TodoChangedEvent e)
        {
            System.Console.WriteLine($"here is {nameof(TodoChangedEvent)}:" + e.Name);
            Thread.Sleep(5000);
            System.Console.WriteLine($"{e.Name} finished!");
        }
    }
}