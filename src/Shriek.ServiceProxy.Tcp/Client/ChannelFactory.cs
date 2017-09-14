using TcpServiceCore.Attributes;
using TcpServiceCore.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TcpServiceCore.Dispatching;
using System.Net.Sockets;
using AspectCore.DynamicProxy;
using AspectCore.Configuration;

namespace TcpServiceCore.Client
{
    public static class ChannelFactory<T>
    {
        private static Type ImplementingType;
        private static Type ProxyType;
        private static ContractDescription<T> Contract;

        static ChannelFactory()
        {
            ImplementingType = typeof(InnerProxy);
            Contract = ContractDescription<T>.Create();
        }

        internal static async Task<T> CreateProxy(Socket socket, ChannelConfig config, bool open = true)
        {
            return await CreateProxy(socket, null, -1, config, open);
        }

        public static async Task<T> CreateProxy(string server, int port, ChannelConfig config, bool open = true)
        {
            return await CreateProxy(null, server, port, config, open);
        }

        private static async Task<T> CreateProxy(Socket socket, string server, int port, ChannelConfig config, bool open)
        {
            if (ProxyType == null)
                ProxyType = CreateProxyType();

            var channelManager = new ChannelManager(Contract, config);

            InnerProxy innerProxy = null;
            if (socket == null)
            {
                innerProxy = new InnerProxy(server, port, channelManager);
            }
            else
            {
                innerProxy = new InnerProxy(socket, channelManager);
            }

            var proxy = Activator.CreateInstance(ProxyType, innerProxy);
            if (open)
                await ((IClientChannel)proxy).Open();

            return (T)proxy;
        }

        private static Type CreateProxyType()
        {
            var _interfaceType = typeof(T);

            if (!_interfaceType.GetTypeInfo().IsInterface)
            {
                throw new InvalidOperationException($"{_interfaceType.FullName} is not an interface");
            }

            var an = new AssemblyName("TcpServiceCore_" + _interfaceType.Name);
            var asm = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);

            var moduleName = Path.ChangeExtension(an.Name, "dll");
            var module = asm.DefineDynamicModule(moduleName);

            var ns = _interfaceType.Namespace;
            if (!string.IsNullOrEmpty(ns))
                ns += ".";
            var builder = module.DefineType(ns + _interfaceType.Name + "_TcpServiceCoreProxy",
                TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit
                | TypeAttributes.AutoClass | TypeAttributes.NotPublic | TypeAttributes.Sealed);

            var innerProxy = builder.DefineField("channel", ImplementingType, FieldAttributes.Private);

            var ctor = builder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.HasThis,
                new Type[] { innerProxy.FieldType });

            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, innerProxy);
            il.Emit(OpCodes.Ret);

            ImplementInterface(typeof(IClientChannel), builder, innerProxy);
            ImplementInterface(_interfaceType, builder, innerProxy);

            return builder.CreateTypeInfo().AsType();
        }

        private static void ImplementInterface(Type interfaceType, TypeBuilder builder, FieldBuilder channel)
        {
            if (builder.FindInterfaces((t, o) => t == interfaceType, null).Length == 0)
            {
                builder.AddInterfaceImplementation(interfaceType);

                var parentInterfaces = interfaceType.GetInterfaces();
                foreach (var _interface in parentInterfaces)
                {
                    ImplementInterface(_interface, builder, channel);
                }

                IEnumerable<OperationDescription> operations = null;
                var intInfo = interfaceType.GetTypeInfo();

                if (ContractDescription.IsContract(intInfo))
                {
                    operations = ContractDescription.ValidateContract(intInfo);
                }
                else
                {
                    operations = interfaceType.GetMethods().Select(x => new OperationDescription(x));
                }

                foreach (var operation in operations)
                {
                    var mb = builder.DefineMethod(
                        $"{interfaceType.Name}.{operation.Name}",
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        operation.ReturnType,
                        operation.ParameterTypes);

                    var il = mb.GetILGenerator();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, channel);

                    MethodInfo invoke = null;
                    //implement the user interface
                    if (operation.IsOperation)
                    {
                        if (operation.IsOneWay)
                        {
                            invoke = ImplementingType.GetMethod("SendOneWay");
                        }
                        else if (operation.IsVoidTask)
                        {
                            invoke = ImplementingType.GetMethod("SendVoid");
                        }
                        else
                        {
                            invoke = ImplementingType.GetMethod("SendReturn")
                                    .MakeGenericMethod(operation.ReturnType.GetGenericArguments()
                                    .First());
                        }

                        il.Emit(OpCodes.Ldstr, mb.Name);
                        il.Emit(OpCodes.Ldc_I4_S, operation.ParameterTypes.Length);
                        il.Emit(OpCodes.Newarr, typeof(object));
                        for (byte x = 0; x < operation.ParameterTypes.Length; x++)
                        {
                            var xType = operation.ParameterTypes[x];
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Ldc_I4_S, x);
                            switch (x)
                            {
                                case 0: il.Emit(OpCodes.Ldarg_1); break;
                                case 1: il.Emit(OpCodes.Ldarg_2); break;
                                case 2: il.Emit(OpCodes.Ldarg_3); break;
                                default: il.Emit(OpCodes.Ldarg_S, x + 1); break;
                            }
                            if (xType.GetTypeInfo().IsValueType)
                                il.Emit(OpCodes.Box, xType);
                            il.Emit(OpCodes.Stelem_Ref);
                        }
                    }
                    else
                    {
                        invoke = ImplementingType.GetMethod(operation.Name, operation.ParameterTypes);
                        for (byte x = 0; x < operation.ParameterTypes.Length; x++)
                        {
                            switch (x)
                            {
                                case 0: il.Emit(OpCodes.Ldarg_1); break;
                                case 1: il.Emit(OpCodes.Ldarg_2); break;
                                case 2: il.Emit(OpCodes.Ldarg_3); break;
                                default: il.Emit(OpCodes.Ldarg_S, x + 1); break;
                            }
                        }
                    }
                    il.Emit(OpCodes.Call, invoke);
                    il.Emit(OpCodes.Ret);

                    builder.DefineMethodOverride(mb, operation.MethodInfo);
                }
            }
        }
    }
}