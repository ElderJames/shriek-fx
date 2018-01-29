using Shriek.ServiceProxy.socket;
using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Exceptions;
using Shriek.ServiceProxy.Socket.Fast.Context;
using Shriek.ServiceProxy.Socket.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Shriek.ServiceProxy.Socket.Fast.Internal
{
    /// <summary>
    /// FastTcp公共类
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// 获取服务类型的Api行为
        /// </summary>
        /// <param name="seviceType">服务类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IEnumerable<ApiAction> GetServiceApiActions(Type seviceType)
        {
            return seviceType
                .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(item => Attribute.IsDefined(item, typeof(ApiAttribute)))
                .Select(method => new ApiAction(method));
        }

        /// <summary>
        /// 设置Api行为返回的任务结果
        /// </summary>
        /// <param name="requestContext">上下文</param>
        /// <param name="taskSetActionTable">任务行为表</param>
        /// <param name="serializer">序列化工具</param>
        /// <returns></returns>
        public static bool SetApiActionTaskResult(RequestContext requestContext, TaskSetterTable<long> taskSetActionTable, ISerializer serializer)
        {
            var taskSetAction = taskSetActionTable.Remove(requestContext.Packet.Id);
            if (taskSetAction == null)
            {
                return true;
            }

            try
            {
                var bytes = requestContext.Packet.Body;
                var value = serializer.Deserialize(bytes, taskSetAction.ValueType);
                return taskSetAction.SetResult(value);
            }
            catch (SerializerException ex)
            {
                return taskSetAction.SetException(ex);
            }
            catch (Exception ex)
            {
                return taskSetAction.SetException(new SerializerException(ex));
            }
        }

        /// <summary>
        /// 设置Api行为返回的任务异常
        /// </summary>
        /// <param name="taskSetActionTable">任务行为表</param>
        /// <param name="requestContext">请求上下文</param>
        /// <returns></returns>
        public static bool SetApiActionTaskException(TaskSetterTable<long> taskSetActionTable, RequestContext requestContext)
        {
            var taskSetAction = taskSetActionTable.Remove(requestContext.Packet.Id);
            if (taskSetAction == null)
            {
                return true;
            }

            var exceptionBytes = requestContext.Packet.Body;
            var message = exceptionBytes == null ? string.Empty : Encoding.UTF8.GetString(exceptionBytes);
            var exception = new RemoteException(message);
            return taskSetAction.SetException(exception);
        }

        /// <summary>
        /// 发送异常信息到远程端
        /// </summary>
        /// <param name="sessionWrapper">会话对象</param>
        /// <param name="exceptionContext">上下文</param>
        /// <returns></returns>
        public static bool SendRemoteException(IWrapper sessionWrapper, ExceptionContext exceptionContext)
        {
            try
            {
                var packet = exceptionContext.Packet;
                packet.IsException = true;
                packet.Body = Encoding.UTF8.GetBytes(exceptionContext.Exception.Message);
                sessionWrapper.UnWrap().Send(packet.ToArraySegment());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 调用远程端的Api
        /// 并返回结果数据任务
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="session">会话对象</param>
        /// <param name="taskSetActionTable">任务行为表</param>
        /// <param name="serializer">序列化工具</param>
        /// <param name="packet">封包</param>
        /// <param name="timeout">超时时间</param>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public static ApiResult<T> InvokeApi<T>(ISession session, TaskSetterTable<long> taskSetActionTable, ISerializer serializer, FastPacket packet, TimeSpan timeout)
        {
            var taskSetter = taskSetActionTable.Create<T>(packet.Id, timeout);
            session.Send(packet.ToArraySegment());
            return new ApiResult<T>(taskSetter);
        }

        public static ApiResult<object> InvokeApi(Type returnType, ISession session, TaskSetterTable<long> taskSetActionTable, ISerializer serializer, FastPacket packet, TimeSpan timeout)
        {
            var taskSetter = taskSetActionTable.Create(returnType, packet.Id, timeout);
            session.Send(packet.ToArraySegment());
            return new ApiResult<object>(taskSetter);
        }

        /// <summary>
        /// 获取和更新ActionContext的参数值
        /// </summary>
        /// <param name="serializer">序列化工具</param>
        /// <param name="actionContext">Api执行上下文</param>
        /// <returns></returns>
        public static object[] GetAndUpdateParameterValues(ISerializer serializer, ActionContext actionContext)
        {
            var parameters = actionContext.Action.Parameters;
            var bodyParameters = actionContext.Packet.GetBodyParameters();

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var value = bodyParameters[i];
                if (value == null || value.Length == 0)
                {
                    parameter.Value = parameter.Type.IsValueType ? Activator.CreateInstance(parameter.Type) : null;
                }
                else
                {
                    parameter.Value = serializer.Deserialize(value, parameter.Type);
                }
            }
            return parameters.Select(p => p.Value).ToArray();
        }
    }
}