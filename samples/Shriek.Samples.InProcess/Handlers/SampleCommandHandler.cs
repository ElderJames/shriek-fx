using System;
using Shriek.Events;
using Shriek.Samples.Events;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Shriek.Commands;
using Shriek.Samples.InProcess.Commands;

namespace Shriek.Samples.InProcess.Handlers
{
    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        public void Execute(ICommandContext context, SampleCommand command)
        {
            var root = context.GetAggregateRoot(command.AggregateId, () => SampleAggregateRoot.Register(command));
            root.Create(command);
        }
    }

    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        public void Handle(SampleEvent e)
        {
            Console.WriteLine($"\tid-{e.No}\ttrrigled\tv{ e.Version}");
            Thread.Sleep(e.Delay);
            Console.WriteLine($"\tid-{e.No}\tevent finished. \tv{e.Version}");
        }
    }
}