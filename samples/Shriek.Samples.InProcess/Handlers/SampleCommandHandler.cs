using Shriek.Commands;
using Shriek.Events;
using Shriek.Samples.InProcess.Commands;
using System;
using System.Threading;

namespace Shriek.Samples.InProcess.Handlers
{
    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        public void Execute(ICommandContext context, SampleCommand command)
        {
            var root = context.GetAggregateRoot(command.AggregateId, () => new SampleAggregateRoot(command.AggregateId, command.No, command.Delay));
            if (!root.CanCommit)
                root.Change(command.AggregateId, command.No, command.Delay);
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