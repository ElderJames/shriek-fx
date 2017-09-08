using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek
{
    internal class ShriekBuilder : IShriekBuilder
    {
        public IServiceCollection Services { get; }

        public ShriekBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}