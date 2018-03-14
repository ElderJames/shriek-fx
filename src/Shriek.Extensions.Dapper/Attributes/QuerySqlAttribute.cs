using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Extensions.Dapper.Attributes
{
    /// <summary>
    /// Define a querying sql on the method in any mapper interface inherited ISqlOperationMapper
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class QuerySqlAttribute : Attribute
    {
        /// <summary>
        /// Sql statement
        /// </summary>
        public string SqlFormat { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sql"></param>
        public QuerySqlAttribute(string sql) => SqlFormat = sql;
    }
}