using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Socket.Tasks
{
    /// <summary>
    /// 表示任务行为
    /// </summary>
    /// <typeparam name="TResult">任务结果类型</typeparam>
    public class TaskSetter<TResult> : ITaskSetter<TResult>, IDisposable
    {
        /// <summary>
        /// 任务源
        /// </summary>
        private readonly TaskCompletionSource<TResult> taskSource;

        /// <summary>
        /// 取消源
        /// </summary>
        private readonly Lazy<CancellationTokenSource> tokenSourceLazy;

        /// <summary>
        /// 获取任务的返回值类型
        /// </summary>
        public Type ValueType
        {
            get
            {
                return typeof(TResult);
            }
        }

        /// <summary>
        /// 任务行为
        /// </summary>
        public TaskSetter()
        {
            this.taskSource = new TaskCompletionSource<TResult>();
            this.tokenSourceLazy = new Lazy<CancellationTokenSource>();
        }

        /// <summary>
        /// 设置任务的行为结果
        /// </summary>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        bool ITaskSetter.SetResult(object value)
        {
            return this.SetResult((TResult)value);
        }

        /// <summary>
        /// 设置任务的行为结果
        /// </summary>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool SetResult(TResult value)
        {
            this.tokenSourceLazy.Value.Dispose();
            return this.taskSource.TrySetResult(value);
        }

        /// <summary>
        /// 设置设置为异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public bool SetException(Exception ex)
        {
            this.tokenSourceLazy.Value.Dispose();
            return this.taskSource.TrySetException(ex);
        }

        /// <summary>
        /// 获取同步结果
        /// </summary>
        /// <returns></returns>
        public TResult GetResult()
        {
            return this.GetTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <returns></returns>
        public Task<TResult> GetTask()
        {
            return this.taskSource.Task;
        }

        /// <summary>
        /// 设置超时时间
        /// 超时后任务产生TimeoutException
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public ITaskSetter<TResult> TimeoutAfter(TimeSpan timeout)
        {
            return this.TimeoutAfter(timeout, (t) => t.SetException(new TimeoutException()));
        }

        /// <summary>
        /// 设置超时时间
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="timeoutAction">超时回调</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public ITaskSetter<TResult> TimeoutAfter(TimeSpan timeout, Action<ITaskSetter<TResult>> timeoutAction)
        {
            if (timeoutAction == null)
            {
                throw new ArgumentNullException("timeoutAction");
            }
            this.tokenSourceLazy.Value.Token.Register(() => timeoutAction(this));
            this.tokenSourceLazy.Value.CancelAfter(timeout);
            return this;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.tokenSourceLazy.IsValueCreated)
            {
                this.tokenSourceLazy.Value.Dispose();
            }
        }
    }
}