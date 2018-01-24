using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shriek.ServiceProxy.Socket.Tasks
{
    /// <summary>
    /// 表示超时任务管理表
    /// 线程安全类型
    /// </summary>
    /// <typeparam name="T">任务ID类型</typeparam>
    [DebuggerDisplay("Count = {table.Count}")]
    public class TaskSetterTable<T>
    {
        /// <summary>
        /// 任务行为字典
        /// </summary>
        private readonly ConcurrentDictionary<T, ITaskSetter> table;

        /// <summary>
        /// 任务行为表
        /// </summary>
        public TaskSetterTable()
        {
            this.table = new ConcurrentDictionary<T, ITaskSetter>();
        }

        /// <summary>
        /// 创建带id的任务并添加到列表中
        /// </summary>
        /// <typeparam name="TResult">任务结果类型</typeparam>
        /// <param name="id">任务id</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public ITaskSetter<TResult> Create<TResult>(T id, TimeSpan timeout)
        {
            var taskSetter = new TaskSetter<TResult>()
                .TimeoutAfter(timeout, (t) => this.Remove(id).SetException(new TimeoutException()));

            this.table.TryAdd(id, taskSetter);
            return taskSetter;
        }

        /// <summary>
        /// 获取并移除与id匹配的任务
        /// 如果没有匹配则返回null
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public ITaskSetter Remove(T id)
        {
            ITaskSetter taskSetter;
            this.table.TryRemove(id, out taskSetter);
            return taskSetter;
        }

        /// <summary>
        /// 取出并移除全部的任务
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITaskSetter> RemoveAll()
        {
            var values = this.table.Values.ToArray();
            this.table.Clear();
            return values;
        }

        /// <summary>
        /// 清除所有任务
        /// </summary>
        public void Clear()
        {
            this.table.Clear();
        }
    }
}