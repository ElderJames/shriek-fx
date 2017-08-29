using System;

namespace Shriek.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string msg) : base(msg)
        {
        }
    }
}