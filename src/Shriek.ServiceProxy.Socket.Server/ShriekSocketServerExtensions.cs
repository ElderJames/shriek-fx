using System;
using System.Linq;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Socket.Server
{
    public static class ShriekSocketServerExtensions
    {
        public static IShriekBuilder AddSocketServer(this IShriekBuilder builder, Action<WebApiProxyOptions> optionAction)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();

            var options = new WebApiProxyOptions();
            optionAction(options);

            foreach (var t in options.RegisteredServices.Select(x => x.Value))
            {
            }

            return builder;
        }
    }
}