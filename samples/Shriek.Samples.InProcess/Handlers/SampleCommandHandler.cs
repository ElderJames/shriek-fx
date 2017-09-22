using System;
using Shriek.Events;
using System.Threading;
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