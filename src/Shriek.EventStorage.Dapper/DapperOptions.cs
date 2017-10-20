using System.Data;

namespace Shriek.EventStorage.Dapper
{
    public class DapperOptions
    {
        public IDbConnection DbConnection { get; set; }
    }
}