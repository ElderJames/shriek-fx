using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Commands;
using Shriek.Domains;
using Shriek.Events;

namespace Shriek.Samples.InProcess.Commands
{
    public class SampleCommand : Command<Guid>
    {
        public SampleCommand(Guid aggregateId) : base(aggregateId)
        { }

        public int No { get; set; }

        public TimeSpan Delay { get; set; }
    }

    public class SampleEvent : Event<Guid>
    {
        public int No { get; set; }

        public TimeSpan Delay { get; set; }
    }

    public class SampleAggregateRoot : AggregateRoot<Guid>,
        IHandle<SampleEvent>
    {
        public int No { get; set; }

        public TimeSpan Delay { get; set; }

        public SampleAggregateRoot() : base(Guid.Empty)
        {
        }

        public SampleAggregateRoot(Guid aggregateId) : base(aggregateId)
        {
        }

        public static SampleAggregateRoot Register(SampleCommand command)
        {
            var root = new SampleAggregateRoot(command.AggregateId);

            return root;
        }

        public void Create(SampleCommand command)
        {
            ApplyChange(new SampleEvent()
            {
                AggregateId = this.AggregateId,
                Version = this.Version,
                No = command.No,
                Delay = command.Delay
            });
        }

        public void Handle(SampleEvent e)
        {
            this.AggregateId = e.AggregateId;
            this.Version = e.Version;
            this.Delay = e.Delay;
            this.No = e.No;
        }
    }
}