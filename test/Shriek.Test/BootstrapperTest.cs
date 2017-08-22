using Shriek.Samples.Aggregates;
using System;
using Shriek.Samples.Commands;
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

            var command = new CreateTodoCommand(id)
            {
                Name = "go to bed",
                Desception = "very good",
                FinishTime = DateTime.Now.AddHours(1)
            };
            bus.Send(command);

            var repository = container.GetService<IRepository<TodoAggregateRoot>>();

            var root = repository.GetById(id);

            Assert.IsNotNull(root);
            Assert.AreEqual(0, root.Version);
            Assert.AreEqual(command.Name, root.Name);
            Assert.AreEqual(command.Desception, root.Desception);

            bus.Send(new ChangeTodoCommand(id)
            {
                Name = "Cho",
                Desception = "Beautiful!"
            });

            root = repository.GetById(id);

            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Version);
            Assert.AreEqual("Cho", root.Name);
            Assert.AreEqual("Beautiful!", root.Desception);
        }
    }
}