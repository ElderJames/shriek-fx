using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Extensions.Dapper
{
    public enum DbActionTypes
    {
        Insert,
        Update,
        Delete,
        DeleteAll,
        Get,
        GetAll
    }
}