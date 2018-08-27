using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Security
{
    public class UserPrincipal : IPrincipal

    {
        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public IIdentity Identity { get; }
    }
}