using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using System;
using System.Threading;
using Shriek.Domains;
using Shriek.Events;
using Shriek.Messages.RabbitMQ;

namespace Shriek.Samples.RabbitMQ
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddShriek()
                .AddRabbitMqEventBus(options =>
                {
                    options.HostName = "localhost";
                    options.UserName = "admin";
                    options.Password = "admin";
                    options.QueueName = "eventBus";
                    options.ExchangeName = "sampleEventBus";
                    options.RouteKey = "sampleEventBus.*";
                })
                .AddRabbitMqCommandBus(options =>
                {
                    options.HostName = "localhost";
                    options.UserName = "admin";
                    options.Password = "admin";
                    options.QueueName = "commandBus";
                    options.ExchangeName = "sampleCommandBus";
                    options.RouteKey = "sampleCommandBus";
                });

            var container = services.BuildServiceProvider();

            var bus = container.GetService<ICommandBus>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var i = 0;
            var j = 0;

            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.D1)
                {
                    try
                    {
                        bus.Send(new SampleCommand(id1)
                        {
                            No = 1,
                            Delay = TimeSpan.FromMilliseconds(5000)
                        });

                        Console.WriteLine($"\tid-1: \tcommand {i++} sended!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:" + ex.Message);
                    }
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    try
                    {
                        bus.Send(new SampleCommand(id2)
                        {
                            No = 2,
                            Delay = TimeSpan.FromMilliseconds(2000)
                        });

                        Console.WriteLine($"\tid-2: \tcommand {j++} sended!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:" + ex.Message);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    #region command and event

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

    #endregion command and event

    #region aggregateRoot

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

    #endregion aggregateRoot

    #region handlers

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

    #endregion handlers
}