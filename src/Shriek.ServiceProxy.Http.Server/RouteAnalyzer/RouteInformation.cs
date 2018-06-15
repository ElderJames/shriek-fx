using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http.Server.RouteAnalyzer
{
    public class RouteInformation
    {
        public string Area { get; set; }
        public string Path { get; set; }
        public string Invocation { get; set; }

        public override string ToString()
        {
            return $"RouteInformation{{Area:\"{Area}\", Path:\"{Path}\", Invocation:\"{Invocation}\"}}";
        }
    }
}