namespace Shriek.ServiceProxy.Http.Server.RouteAnalyzer
{
    public class RouteInformation
    {
        public string HttpMethod { get; set; }
        public string Area { get; set; }
        public string Path { get; set; }
        public string Invocation { get; set; }

        public override string ToString()
        {
            return $"RouteInformation{{ Method:\"{HttpMethod}\" Area:\"{Area}\", Path:\"{Path}\", Invocation:\"{Invocation}\"}}";
        }
    }
}