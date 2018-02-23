using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Core.Internal;
using Shriek.ServiceProxy.Socket.Exceptions;
using Shriek.ServiceProxy.Socket.Fast.Context;
using Shriek.ServiceProxy.Socket.Fast.Internal;
using Shriek.ServiceProxy.Socket.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Reflection;

namespace Shriek.ServiceProxy.Socket.Fast
{
    /// <summary>
    /// 表示Fast协议的tcp客户端
    /// </summary>
    public class FastTcpClient : TcpClientBase
    {
        /// <summary>
        /// 所有Api行为
        /// </summary>
        private ApiActionTable apiActionTable;

        /// <summary>
        /// 数据包id提供者
        /// </summary>
        private PacketIdProvider packetIdProvider;

        /// <summary>
        /// 任务行为表
        /// </summary>
        private TaskSetterTable<long> taskSetterTable;

        /// <summary>
        /// 获取或设置序列化工具
        /// 默认是Json序列化
        /// </summary>
        public ISerializer Serializer { get; set; }

        /// <summary>
        /// 获取或设置请求等待超时时间(毫秒)
        /// 默认30秒
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan TimeOut { get; set; }

        /// <summary>
        /// Fast协议的tcp客户端
        /// </summary>
        public FastTcpClient()
        {
            this.Init();
        }

        /// <summary>
        /// SSL支持的Fast协议的tcp客户端
        /// </summary>
        /// <param name="targetHost">目标主机</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FastTcpClient(string targetHost)
            : base(targetHost)
        {
            this.Init();
        }

        /// <summary>
        /// SSL支持的Fast协议的tcp客户端
        /// </summary>
        /// <param name="targetHost">目标主机</param>
        /// <param name="certificateValidationCallback">远程证书验证回调</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FastTcpClient(string targetHost, RemoteCertificateValidationCallback certificateValidationCallback)
            : base(targetHost, certificateValidationCallback)
        {
            this.Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.apiActionTable = new ApiActionTable(Common.GetServiceApiActions(this.GetType()));
            this.packetIdProvider = new PacketIdProvider();
            this.taskSetterTable = new TaskSetterTable<long>();
            this.Serializer = new DefaultSerializer();
            this.TimeOut = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// 当接收到远程端的数据时，将触发此方法
        /// </summary>
        /// <param name="streamReader">数据读取器</param>
        /// <returns></returns>
        protected sealed override Task OnReceiveAsync(ISessionStreamReader streamReader)
        {
            var packages = this.GenerateFastPackets(streamReader);
            foreach (var package in packages)
            {
                this.ProcessPacketAsync(package);
            }
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 生成数据包
        /// </summary>
        /// <param name="streamReader">数据流</param>
        /// <returns></returns>
        private IList<FastPacket> GenerateFastPackets(ISessionStreamReader streamReader)
        {
            var list = new List<FastPacket>();
            while (true)
            {
                var packet = default(FastPacket);
                if (FastPacket.Parse(streamReader, out packet) == false)
                {
                    return list;
                }
                if (packet == null)
                {
                    return list;
                }
                list.Add(packet);
            }
        }

        /// <summary>
        /// 处理接收到服务发来的数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        private async void ProcessPacketAsync(FastPacket packet)
        {
            var requestContext = new RequestContext(null, packet, null);
            if (packet.IsException == true)
            {
                Common.SetApiActionTaskException(this.taskSetterTable, requestContext);
            }
            else if (packet.IsFromClient == true)
            {
                Common.SetApiActionTaskResult(requestContext, this.taskSetterTable, this.Serializer);
            }
            else
            {
                await TryProcessRequestPackageAsync(requestContext);
            }
        }

        /// <summary>
        /// 处理服务器请求的数据包
        /// </summary>
        /// <param name="requestContext">上下文</param>
        /// <returns></returns>
        private async Task TryProcessRequestPackageAsync(RequestContext requestContext)
        {
            try
            {
                var action = this.GetApiAction(requestContext);
                var actionContext = new ActionContext(requestContext, action);
                await this.ExecuteActionAsync(actionContext);
            }
            catch (Exception ex)
            {
                var exceptionContext = new ExceptionContext(requestContext, ex);
                Common.SendRemoteException(this, exceptionContext);
                this.OnException(requestContext.Packet, ex);
            }
        }

        /// <summary>
        /// 获取Api行为
        /// </summary>
        /// <param name="requestContext">请求上下文</param>
        /// <exception cref="ApiNotExistException"></exception>
        /// <returns></returns>
        private ApiAction GetApiAction(RequestContext requestContext)
        {
            var action = this.apiActionTable.TryGetAndClone(requestContext.Packet.ApiName);
            if (action != null)
            {
                return action;
            }
            throw new ApiNotExistException(requestContext.Packet.ApiName);
        }

        /// <summary>
        /// 执行Api行为
        /// </summary>
        /// <param name="actionContext">上下文</param>
        /// <returns></returns>
        private async Task ExecuteActionAsync(ActionContext actionContext)
        {
            var action = actionContext.Action;
            var parameters = Common.GetAndUpdateParameterValues(this.Serializer, actionContext);
            var result = await action.ExecuteAsync(this, parameters);

            if (action.IsVoidReturn == false && this.IsConnected == true)
            {
                actionContext.Packet.Body = this.Serializer.Serialize(result);
                this.TrySendPackage(actionContext.Packet);
            }
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns></returns>
        private bool TrySendPackage(FastPacket package)
        {
            try
            {
                return this.Send(package.ToArraySegment()) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///  当操作中遇到处理异常时，将触发此方法
        /// </summary>
        /// <param name="packet">数据包对象</param>
        /// <param name="exception">异常对象</param>
        protected virtual void OnException(FastPacket packet, Exception exception)
        {
        }

        /// <summary>
        /// 调用服务端实现的Api
        /// </summary>
        /// <param name="api">Api行为的api</param>
        /// <param name="parameters">参数列表</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="SerializerException"></exception>
        public void InvokeApi(string api, params object[] parameters)
        {
            var packet = new FastPacket(api, this.packetIdProvider.NewId(), true);
            packet.SetBodyParameters(this.Serializer, parameters);
            this.Send(packet.ToArraySegment());
        }

        /// <summary>
        /// 调用服务端实现的Api
        /// 并返回结果数据任务
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="api">Api行为的api</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="SerializerException"></exception>
        /// <returns>远程数据任务</returns>
        public ApiResult<T> InvokeApi<T>(string api, params object[] parameters)
        {
            var id = this.packetIdProvider.NewId();
            var packet = new FastPacket(api, id, true);
            packet.SetBodyParameters(this.Serializer, parameters);
            return Common.InvokeApi<T>(this.UnWrap(), this.taskSetterTable, this.Serializer, packet, this.TimeOut);
        }

        public ApiResult<object> InvokeApi(Type returnType, string api, params object[] parameters)
        {
            var id = this.packetIdProvider.NewId();
            var packet = new FastPacket(api, id, true);
            packet.SetBodyParameters(this.Serializer, parameters);
            return Common.InvokeApi(returnType, this.UnWrap(), this.taskSetterTable, this.Serializer, packet, this.TimeOut);
        }

        /// <summary>
        /// 断开时清除数据任务列表
        /// </summary>
        protected override void OnDisconnected()
        {
            var taskSetActions = this.taskSetterTable.RemoveAll();
            foreach (var taskSetAction in taskSetActions)
            {
                var exception = new SocketException(SocketError.Shutdown.GetHashCode());
                taskSetAction.SetException(exception);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.apiActionTable = null;
            this.taskSetterTable.Clear();
            this.taskSetterTable = null;
            this.packetIdProvider = null;
            this.Serializer = null;
        }
    }
}