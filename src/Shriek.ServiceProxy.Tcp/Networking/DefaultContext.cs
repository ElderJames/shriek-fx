namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示上下文对象
    /// </summary>
    internal class DefaultContext : IContenxt
    {
        /// <summary>
        /// 获取或设置当前会话对象
        /// </summary>
        public ISession Session { get; set; }

        /// <summary>
        /// 获取当前会话收到的数据读取器
        /// </summary>
        public ISessionStreamReader StreamReader { get; set; }

        /// <summary>
        /// 获取或设置所有会话对象
        /// </summary>
        public ISessionManager AllSessions { get; set; }
    }
}