using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Shriek.ServiceProxy.Http.Server.RouteAnalyzer.Impl
{
    internal class Router : IRouter
    {
        private readonly IRouter defaultRouter;
        internal readonly string RoutePath;

        public Router(IRouter defaultRouter, string routePath)
        {
            this.defaultRouter = defaultRouter;
            this.RoutePath = routePath;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {
            if (context.HttpContext.Request.Path == RoutePath)
            {
                var routeData = new RouteData(context.RouteData);
                routeData.Routers.Add(this.defaultRouter);
                routeData.Values["controller"] = "RouteAnalyzer";
                routeData.Values["action"] = "ShowAllRoutes";
                context.RouteData = routeData;
                await this.defaultRouter.RouteAsync(context);
            }
        }
    }
}