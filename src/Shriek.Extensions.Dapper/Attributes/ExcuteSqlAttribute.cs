using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Extensions.Dapper.Attributes
{
    /// <summary>
    /// Define an excuting sql on the method in any mapper interface inherited ISqlOperationMapper
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExcuteSqlAttribute : Attribute
    {
        /// <summary>
        /// Sql statement
        /// </summary>
        public string SqlFormat { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sql"></param>
        public ExcuteSqlAttribute(string sql) => SqlFormat = sql;
    }
}