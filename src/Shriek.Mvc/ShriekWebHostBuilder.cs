using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.Mvc
{
    public class ShriekWebHostBuilder : WebHostBuilder, IShriekBuilder
    {
        public IServiceCollection Services { get; }
    }
}