using System;

namespace Shriek.ServiceProxy.Tcp.Exceptions
{
    public class ExceptionHandler
    {
        public virtual void LogException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine(ex.InnerException.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}