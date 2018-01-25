using System;
using System.Linq;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Socket.Fast;

namespace Shriek.ServiceProxy.Socket.Server
{
    public static class ShriekSocketServerExtensions
    {
        public static IShriekBuilder AddSocketServer(this IShriekBuilder builder, Action<WebApiProxyOptions> optionAction)
        {
            AppDomain.CurrentDomain.UpdateExcutingAssemblies();

            var options = new WebApiProxyOptions();
            optionAction(options);
            var listener = new TcpListener();
            var middleware = listener.Use<FastMiddleware>();

            foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(x => !x.IsInterface))
            {
                var serviceTypes = options.RegisteredServices.Where(x => x.Value.IsAssignableFrom(type));
                if (serviceTypes.Any())
                {
                    middleware.BindService(type);
                }
            }

            listener.Start(options.Port);
            return builder;
        }
    }
}