using System;

namespace Shriek.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string msg) : base(msg)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}