using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 表示Api行为表
    /// </summary>
    internal class ApiActionTable
    {
        /// <summary>
        /// Api行为字典
        /// </summary>
        private readonly Dictionary<string, ApiAction> dictionary;

        /// <summary>
        /// Api行为表
        /// </summary>
        public ApiActionTable()
        {
            this.dictionary = new Dictionary<string, ApiAction>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Api行为列表
        /// </summary>
        /// <param name="apiActions">Api行为</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ApiActionTable(IEnumerable<ApiAction> apiActions)
            : this()
        {
            this.AddRange(apiActions);
        }

        /// <summary>
        /// 添加Api行为
        /// </summary>
        /// <param name="apiAction">Api行为</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(ApiAction apiAction)
        {
            if (apiAction == null)
            {
                throw new ArgumentNullException("apiAction");
            }

            if (this.dictionary.ContainsKey(apiAction.ApiName))
            {
                throw new ArgumentException(string.Format("Api行为{0}或其命令值已存在", apiAction.ApiName));
            }

            this.dictionary.Add(apiAction.ApiName, apiAction);
        }

        /// <summary>
        /// 添加Api行为
        /// </summary>
        /// <param name="apiActions">Api行为</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddRange(IEnumerable<ApiAction> apiActions)
        {
            foreach (var action in apiActions)
            {
                this.Add(action);
            }
        }

        /// <summary>
        /// 获取Api行为
        /// 如果获取不到则返回null
        /// </summary>
        /// <param name="name">行为名称</param>
        /// <returns></returns>
        public ApiAction TryGetAndClone(string name)
        {
            ApiAction apiAction;
            if (this.dictionary.TryGetValue(name, out apiAction))
            {
                return ((ICloneable<ApiAction>)apiAction).CloneConstructor();
            }
            return null;
        }
    }
}