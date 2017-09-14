using TcpServiceCore.Communication;
using TcpServiceCore.Protocol;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpServiceCore.Tools;
using TcpServiceCore.Dispatching;
using AspectCore.DynamicProxy;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Tcp;

namespace TcpServiceCore.Client
{
    public class InnerProxy : CommunicationObject, IInterceptor, IClientChannel, IServiceClient
    {
        private readonly string server;
        private readonly int port;
        private readonly Socket socket;
        private readonly AsyncStreamHandler streamHandler;
        private readonly ChannelManager channelManager;
        private readonly IMsgIdProvider idProvider;
        private readonly string contract;
        private readonly bool isNew;

        public bool AllowMultiple => throw new NotImplementedException();

        public bool Inherited { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Order { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IJsonFormatter JsonFormatter => throw new NotImplementedException();

        public Uri RequestHost => throw new NotImplementedException();

        internal InnerProxy(Socket socket, ChannelManager channelManager, bool open = false)
        {
            this.idProvider = Global.IdProvider;

            this.socket = socket;
            if (this.socket == null)
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.channelManager = channelManager;

            this.contract = this.channelManager.Contract.ContractName;

            this.socket.Configure(this.channelManager.Config);

            this.streamHandler = new AsyncStreamHandler(this.socket, this.channelManager.BufferManager);

            if (open)
                this.Open().Wait();
        }

        public InnerProxy(string server, int port, ChannelManager channelManager, bool open = false)
            : this(null, channelManager, open)
        {
            this.server = server;
            this.port = port;
            isNew = true;
        }

        protected override async Task OnOpen()
        {
            if (this.isNew)
                await socket.ConnectAsync(server, port);
            await this.streamHandler.Open();
        }

        protected override async Task OnClose()
        {
            await this.streamHandler.Close();
            socket.Dispose();
        }

        public Task SendOneWay(string method, params object[] msg)
        {
            var request = new Message(MessageType.Request, 0, this.contract, method, msg);
            return this.streamHandler.WriteMessage(request);
        }

        public Task SendVoid(string method, params object[] msg)
        {
            var request = this.CreateRequest(method, msg);
            return this.streamHandler.WriteRequest(request, this.socket.ReceiveTimeout);
        }

        public async Task<R> SendReturn<R>(string method, params object[] msg)
        {
            var request = this.CreateRequest(method, msg);
            var response = await this.streamHandler.WriteRequest(request, this.socket.ReceiveTimeout);
            if (response.MessageType == MessageType.Error)
                throw new Exception(Global.Serializer.Deserialize<string>(response.Parameters[0]));
            var result = Global.Serializer.Deserialize<R>(response.Parameters[0]);
            return result;
        }

        private Message CreateRequest(string method, params object[] msg)
        {
            var id = int.Parse(this.idProvider.NewId());
            return new Message(MessageType.Request, id, this.contract, method, msg);
        }

        public async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var _context = AspectCoreContext.From(context);

            var actionContext = new TcpActionContext()
            {
                HttpApiClient = this,
                RouteAttributes = _context.RouteAttributes,
                ApiReturnAttribute = _context.ApiReturnAttribute,
                ApiActionFilterAttributes = _context.ApiActionFilterAttributes,
                ApiActionDescriptor = _context.ApiActionDescriptor.Clone() as ApiActionDescriptor
            };

            var parameters = actionContext.ApiActionDescriptor.Parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Value = context.Parameters[i];
            }

            var apiAction = _context.ApiActionDescriptor;

            await next(context);

            context.ReturnValue = apiAction.Execute(actionContext);
        }

        public Task SendAsync(ApiActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}