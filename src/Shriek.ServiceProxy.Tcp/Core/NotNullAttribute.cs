using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 表示Api行为的参数值不能为NULL验证标记
    /// </summary>
    public sealed class NotNullAttribute : ParameterFilterAttribute
    {
        /// <summary>
        /// Api执行之前
        /// 在此检测parameter的输入合理性
        /// 不合理可以抛出异常
        /// </summary>
        /// <param name="action">关联的Api行为</param>
        /// <param name="parameter">参数信息</param>
        public override void OnExecuting(ApiAction action, ApiParameter parameter)
        {
            if (parameter.Value == null)
            {
                throw new ArgumentNullException(parameter.Name);
            }
        }
    }
}
