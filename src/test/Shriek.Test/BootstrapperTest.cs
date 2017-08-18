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

            var container = services.BuildServiceProvider();

            var bus = container.GetService<ICommandBus>();

            Assert.IsNotNull(bus);

            var id = Guid.NewGuid();

            var command = new CreateConfigItemCommand(id)
            {
                Name = "ysj",
                Value = "very good"
            };
            bus.Send(command);

            var repository = container.GetService<IRepository<ConfigItemAggregateRoot>>();

            var root = repository.GetById(id);

            Assert.IsNotNull(root);
            Assert.AreEqual(0, root.Version);
            Assert.AreEqual(command.Name, root.Name);
            Assert.AreEqual(command.Value, root.Value);

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Cho",
                Value = "Beautiful!"
            });

            root = repository.GetById(id);

            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Version);
            Assert.AreEqual("Cho", root.Name);
            Assert.AreEqual("Beautiful!", root.Value);
        }
    }
}