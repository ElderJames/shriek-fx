using Shriek.Commands;
using Shriek.Domains;
using Shriek.Events;
using System;

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
        public int No { get; private set; }

        public TimeSpan Delay { get; private set; }

        public SampleAggregateRoot(Guid aggregateId, int no, TimeSpan delay) : base(aggregateId)
        {
            ApplyChange(new SampleEvent()
            {
                AggregateId = this.AggregateId,
                No = no,
                Delay = delay
            });
        }

        public void Change(Guid aggregateId, int no, TimeSpan delay)
        {
            ApplyChange(new SampleEvent()
            {
                AggregateId = this.AggregateId,
                No = no,
                Delay = delay
            });
        }

        public void Handle(SampleEvent e)
        {
            this.AggregateId = e.AggregateId;
            this.Delay = e.Delay;
            this.No = e.No;
        }
    }
}