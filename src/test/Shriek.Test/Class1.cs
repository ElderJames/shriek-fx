using Shriek.Commands;
using System.Runtime.InteropServices.ComTypes;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var bus = services.BuildServiceProvider().GetService<ICommandBus>();

            Assert.IsNotNull(bus);
        }
    }
}