using Shriek.ServiceProxy.socket;
using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Core.Internal;
using Shriek.ServiceProxy.Socket.Exceptions;
using Shriek.ServiceProxy.Socket.Fast.Context;
using Shriek.ServiceProxy.Socket.Fast.Internal;
using Shriek.ServiceProxy.Socket.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Socket.Fast
{
    /// <summary>
    /// 表示fast协议中间件
    /// </summary>
    public class FastMiddleware : IMiddleware, IDependencyResolverSupportable, IFilterSupportable
    {
        /// <summary>
        /// 所有Api行为
        /// </summary>
        private readonly ApiActionTable apiActionTable;

        /// <summary>
        /// 获取数据包id提供者
        /// </summary>
        internal PacketIdProvider PacketIdProvider { get; private set; }

        /// <summary>
        /// 获取任务行为记录表
        /// </summary>
        internal TaskSetterTable<long> TaskSetterTable { get; private set; }

        /// <summary>
        /// 获取或设置请求等待超时时间(毫秒)
        /// 默认30秒
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan TimeOut { get; set; }

        /// <summary>
        /// 下一个中间件
        /// </summary>
        public IMiddleware Next { get; set; }

        /// <summary>
        /// 获取或设置序列化工具
        /// 默认提供者是Json序列化
        /// </summary>
        public ISerializer Serializer { get; set; }

        /// <summary>
        /// 获取全局过滤器管理者
        /// </summary>
        public IGlobalFilters GlobalFilters { get; private set; }

        /// <summary>
        /// 获取或设置依赖关系解析提供者
        /// 默认提供者解析为单例模式
        /// </summary>
        public IDependencyResolver DependencyResolver { get; set; }

        /// <summary>
        /// 获取或设置Api行为特性过滤器提供者
        /// </summary>
        public IFilterAttributeProvider FilterAttributeProvider { get; set; }

        /// <summary>
        /// fast协议中间件
        /// </summary>
        public FastMiddleware()
        {
            this.apiActionTable = new ApiActionTable();
            this.PacketIdProvider = new PacketIdProvider();
            this.TaskSetterTable = new TaskSetterTable<long>();

            this.TimeOut = TimeSpan.FromSeconds(30);
            this.Serializer = new DefaultSerializer();
            this.GlobalFilters = new FastGlobalFilters();
            this.DependencyResolver = new DefaultDependencyResolver();
            this.FilterAttributeProvider = new DefaultFilterAttributeProvider();

            DomainAssembly.GetAssemblies().ForEach(this.BindService);
        }

        /// <summary>
        /// 绑定程序集下所有实现IFastApiService的服务
        /// </summary>
        /// <param name="assembly">程序集</param>
        private void BindService(Assembly assembly)
        {
            var fastApiServices = assembly.GetTypes().Where(item =>
                item.IsAbstract == false
                && item.IsInterface == false
                && typeof(IFastApiService).IsAssignableFrom(item));

            foreach (var type in fastApiServices)
            {
                var actions = Common.GetServiceApiActions(type);
                this.apiActionTable.AddRange(actions);
            }
        }

        public void BindService(Type serviceType)
        {
            var actions = Common.GetServiceApiActions(serviceType);
            this.apiActionTable.AddRange(actions);
        }

        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task IMiddleware.Invoke(IContenxt context)
        {
            var protocal = context.Session.Protocol;
            if (protocal != Protocol.None && protocal != Protocol.Fast)
            {
                return this.Next.Invoke(context);
            }
            return this.OnFastRequestAsync(context);
        }

        /// <summary>
        /// 收到fast请求
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private Task OnFastRequestAsync(IContenxt context)
        {
            var fastPacket = default(FastPacket);
            if (FastPacket.Parse(context.StreamReader, out fastPacket) == false)
            {
                return this.Next.Invoke(context);
            }

            // 数据未完整
            if (fastPacket == null)
            {
                var wrapper = new FastSession(context.Session, this);
                context.Session.SetProtocolWrapper(Protocol.Fast, wrapper);
                return TaskExtend.CompletedTask;
            }

            if (context.Session.Protocol == Protocol.None)
            {
                var wrapper = new FastSession(context.Session, this);
                context.Session.SetProtocolWrapper(Protocol.Fast, wrapper);
            }

            var fastSession = (FastSession)context.Session.Wrapper;
            var fastPackets = this.GenerateFastPackets(context, fastPacket);

            foreach (var packet in fastPackets)
            {
                var requestContext = new RequestContext(fastSession, packet, context.AllSessions);
                this.OnRecvFastPacketAsync(requestContext);
            }
            return TaskExtend.CompletedTask;
        }

        /// <summary>
        /// 生成数据包
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="fastPacket">数据包</param>
        /// <returns></returns>
        private IList<FastPacket> GenerateFastPackets(IContenxt context, FastPacket fastPacket)
        {
            var list = new List<FastPacket> { fastPacket };
            while (true)
            {
                var packet = default(FastPacket);
                if (FastPacket.Parse(context.StreamReader, out packet) == false)
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
        /// 接收到会话对象的数据包
        /// </summary>
        /// <param name="requestContext">请求上下文</param>
        private async void OnRecvFastPacketAsync(RequestContext requestContext)
        {
            if (requestContext.Packet.IsException == true)
            {
                Common.SetApiActionTaskException(this.TaskSetterTable, requestContext);
            }
            else
            {
                await this.ProcessRequestAsync(requestContext);
            }
        }

        /// <summary>
        /// 处理正常的数据请求
        /// </summary>
        /// <param name="requestContext">请求上下文</param>
        /// <returns></returns>
        private async Task ProcessRequestAsync(RequestContext requestContext)
        {
            if (requestContext.Packet.IsFromClient == false)
            {
                Common.SetApiActionTaskResult(requestContext, this.TaskSetterTable, this.Serializer);
            }
            else
            {
                await this.TryExecuteRequestAsync(requestContext);
            }
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="requestContext">上下文</param>
        /// <returns></returns>
        private async Task TryExecuteRequestAsync(RequestContext requestContext)
        {
            try
            {
                var action = this.GetApiAction(requestContext);
                var actionContext = new ActionContext(requestContext, action);
                var fastApiService = this.GetFastApiService(actionContext);
                await fastApiService.ExecuteAsync(actionContext);
                this.DependencyResolver.TerminateService(fastApiService);
            }
            catch (Exception ex)
            {
                var context = new ExceptionContext(requestContext, ex);
                this.OnException(requestContext.Session, context);
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
            if (action == null)
            {
                throw new ApiNotExistException(requestContext.Packet.ApiName);
            }
            return action;
        }

        /// <summary>
        /// 获取FastApiService实例
        /// </summary>
        /// <param name="actionContext">Api行为上下文</param>
        /// <exception cref="ResolveException"></exception>
        /// <returns></returns>
        private IFastApiService GetFastApiService(ActionContext actionContext)
        {
            try
            {
                var serviceType = actionContext.Action.DeclaringService;
                var fastApiService = this.DependencyResolver.GetService(serviceType) as FastApiService;
                return fastApiService.Init(this);
            }
            catch (Exception ex)
            {
                throw new ResolveException(actionContext.Action.DeclaringService, ex);
            }
        }

        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="sessionWrapper">产生异常的会话</param>
        /// <param name="context">上下文</param>
        protected virtual void OnException(IWrapper sessionWrapper, ExceptionContext context)
        {
            Common.SendRemoteException(sessionWrapper, context);
        }
    }
}