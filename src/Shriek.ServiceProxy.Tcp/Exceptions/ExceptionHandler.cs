using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServiceCore.Exceptions
{
    public class ExceptionHandler
    {
        public virtual void LogException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if(ex.InnerException != null)
                Console.WriteLine(ex.InnerException.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
