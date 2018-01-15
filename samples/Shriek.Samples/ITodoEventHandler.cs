using Newtonsoft.Json;
using Shriek.Samples.Events;
using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Samples
{
    public class ITodoEventHandler : IEventHandler<TodoCreatedEvent>
    {
        public void Handle(TodoCreatedEvent e)
        {
            Console.WriteLine("other assembly's handler:" + JsonConvert.SerializeObject(e));
        }
    }
}