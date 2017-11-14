namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义克隆相关的接口
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface ICloneable<out T>
    {
        /// <summary>
        /// 克隆构造器行为
        /// </summary>
        /// <returns></returns>
        T CloneConstructor();
    }
}