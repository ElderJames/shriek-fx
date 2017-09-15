using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Tcp.Communication;
using Shriek.ServiceProxy.Tcp.Dispatching;
using Shriek.ServiceProxy.Tcp.Protocol;
using Shriek.ServiceProxy.Tcp.Tools;

namespace Shriek.ServiceProxy.Tcp.Client
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

        public bool AllowMultiple => false;

        public bool Inherited { get; set; }
        public int Order { get; set; }

        public IJsonFormatter JsonFormatter => new DefaultJsonFormatter();

        public Uri RequestHost { get; set; }

        internal InnerProxy(Socket socket, ChannelManager channelManager, bool open)
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
                Open().Wait();
        }

        public InnerProxy(string server, int port, ChannelManager channelManager, bool open)
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
                RequestMessage = this.CreateRequest(_context.ApiActionDescriptor.Name),
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

        public async Task SendAsync(ApiActionContext context)
        {
            if (!(context is TcpActionContext tcpContext)) return;

            var request = this.CreateRequest(tcpContext.ApiActionDescriptor.Name, tcpContext.ApiActionDescriptor.Parameters.Select(x => x.Value));
            tcpContext.ResponseMessage = await this.streamHandler.WriteRequest(request, this.socket.ReceiveTimeout);
            //if (tcpContext.ResponseMessage.MessageType == MessageType.Error)
            //    throw new Exception(Global.Serializer.Deserialize<string>(response.Parameters[0]));
            //var result = Global.Serializer.Deserialize(tcpContext.ApiActionDescriptor.ReturnDataType, response.Parameters[0]);
        }
    }
}