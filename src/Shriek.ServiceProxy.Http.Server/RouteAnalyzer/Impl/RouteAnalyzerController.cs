using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Shriek.ServiceProxy.Http.Server.RouteAnalyzer.Impl
{
    [Controller]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RouteAnalyzerController
    {
        private readonly IRouteAnalyzer _routeAnalyzer;

        public RouteAnalyzerController(IRouteAnalyzer routeAnalyzer)
        {
            _routeAnalyzer = routeAnalyzer;
        }

        public string ShowAllRoutes()
        {
            var infos = _routeAnalyzer.GetAllRouteInformations();
            return infos.Aggregate("", (current, info) => current + (info.ToString() + "\n"));
        }
    }
}