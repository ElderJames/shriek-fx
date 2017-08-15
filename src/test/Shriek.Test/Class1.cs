using Shriek.ConfigCenter.Domain.Aggregates;
using System;
using Shriek.ConfigCenter.Domain.Commands;
using Shriek.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shriek.Storage;

namespace Shriek.Test
{
    [TestClass]
    public class TestDI
    {
        [TestMethod]
        public void BootstrapperTest()
        {
            var services = new ServiceCollection();

            services.AddShriek();
            services.AddScoped<IRepository<ConfigItemAggregateRoot>, Repository<ConfigItemAggregateRoot>>();
            services.AddSingleton<IEventStorage, InMemoryEventStorage>();

            var bus = services.BuildServiceProvider().GetService<ICommandBus>();

            Assert.IsNotNull(bus);

            var id = Guid.NewGuid();

            var command = new CreateConfigItemCommand(id)
            {
                Name = "ysj",
                Value = "very good"
            };
            bus.Send(command);

            var repository = services.BuildServiceProvider().GetService<IRepository<ConfigItemAggregateRoot>>();

            var root = repository.GetById(id);

            Assert.IsNotNull(root);
            Assert.Equals(command.Name, root.Name);
            Assert.Equals(command.Value, root.Value);
        }
    }
}