using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shriek.Commands;
using Shriek.Events;
using Shriek.Messages;
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

            var commandBus = container.GetService<ICommandBus>();
            var eventBus = container.GetService<IEventBus>();
            var eventStorage = container.GetService<IEventStorage>();
            var messageProcessor = container.GetService<IMessagePublisher>();

            Assert.IsNotNull(commandBus);
            Assert.IsNotNull(eventBus);
            Assert.IsNotNull(eventStorage);
            Assert.IsNotNull(messageProcessor);
        }
    }
}