using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class SampleTestService : Samples.Services.ITestService
    {
        public string Test(string name)
        {
            return name;
        }
    }
}