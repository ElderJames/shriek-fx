using Shriek.ServiceProxy.Socket.Reflection;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Socket.Tasks
{
    /// <summary>
    /// 提供Task的扩展
    /// </summary>
    public static class TaskExtend
    {
        /// <summary>
        /// 表示已完成的task
        /// </summary>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);

        /// <summary>
        /// Task的Result getter缓存
        /// </summary>
        private readonly static ConcurrentDictionary<Type, PropertyGetter> cache = new ConcurrentDictionary<Type, PropertyGetter>();

        /// <summary>
        /// 转换为TaskOf(TResult)类型
        /// </summary>
        /// <typeparam name="TResult">目标Task的Result的类型</typeparam>
        /// <param name="sourceTask">源Task</param>
        /// <returns></returns>
        public static Task<TResult> Cast<TResult>(this Task sourceTask)
        {
            return sourceTask.Cast<TResult>(sourceTask.GetType());
        }

        /// <summary>
        /// 转换为TaskOf(TResult)类型
        /// </summary>
        /// <typeparam name="TResult">目标Task的Result的类型</typeparam>
        /// <param name="sourceTask">源Task</param>
        /// <param name="sourceTaskType">源Task的类型</param>
        /// <returns></returns>
        public async static Task<TResult> Cast<TResult>(this Task sourceTask, Type sourceTaskType)
        {
            await sourceTask;
            var property = cache.GetOrAdd(sourceTaskType, type => new PropertyGetter(type, "Result"));
            return (TResult)property.Invoke(sourceTask);
        }
    }
}