using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Tcp.Buffering;
using Shriek.ServiceProxy.Tcp.Communication;

namespace Shriek.ServiceProxy.Tcp.Protocol
{
    internal abstract class StreamHandler : CommunicationObject
    {
        private readonly ConcurrentDictionary<int, ResponseEvent> mapper = new ConcurrentDictionary<int, ResponseEvent>();

        protected readonly Socket Socket;

        protected IBufferManager BufferManager { get; set; }

        protected StreamHandler(Socket socket, IBufferManager bufferManager)
        {
            this.Socket = socket;
            this.BufferManager = bufferManager;
        }

        public virtual bool Connected => this.Socket.Connected;

        protected abstract Task _Write(ArraySegment<byte> buffer);

        protected abstract Task<int> _Read(ArraySegment<byte> buffer);

        protected virtual Task _OnRequestReceived(Message request)
        {
            return Task.CompletedTask;
        }

        protected override Task OnOpen()
        {
            Task.Run(async () =>
            {
                while (this.State == CommunicationState.Openning)
                {
                    //waiting the open state.
                }
                while (this.State == CommunicationState.Opened)
                {
                    var msg = await this.ReadMessage();
                    switch (msg.MessageType)
                    {
                        case MessageType.Response:
                        case MessageType.Error:
                            this.mapper.TryRemove(msg.Id, out var responseEvent);
                            responseEvent.SetResponse(msg);
                            break;

                        case MessageType.Request:
                            await this._OnRequestReceived(msg);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(msg.MessageType));
                    }
                }
            });
            return Task.CompletedTask;
        }

        protected override Task OnClose()
        {
            return Task.CompletedTask;
        }

        public async Task<Message> ReadMessage()
        {
            this.ThrowIfNotOpened();

            var size = await this.GetMessageSize();

            var poolBuffer = this.BufferManager.GetFitBuffer(size);

            var buffer = new ArraySegment<byte>(poolBuffer, 0, size);

            var index = 0;

            var segment = await this.ReadBytes(buffer);

            var data = segment.Array;

            var msgType = (MessageType)data[index];

            index += 1;

            var id = BitConverter.ToInt32(data, index);

            index += 4;

            var contractLength = data[index];

            index += 1;

            var contract = Encoding.ASCII.GetString(data, index, contractLength);

            index += contractLength;

            var methodLength = data[index];

            index += 1;

            var method = Encoding.ASCII.GetString(data, index, methodLength);

            index += methodLength;

            var parametersCount = data[index];

            index += 1;

            var load = new byte[parametersCount][];

            for (byte i = 0; i < parametersCount; i++)
            {
                var paramLength = BitConverter.ToInt32(data, index);

                index += 4;

                var param = data.Skip(index).Take(paramLength).ToArray();

                index += paramLength;

                load[i] = param;
            }

            var request = new Message(msgType, id, contract, method, load);

            this.BufferManager.AddBuffer(data);

            return request;
        }

        public async Task WriteMessage(Message request)
        {
            this.ThrowIfNotOpened();

            var data = new List<byte> { (byte)request.MessageType };

            data.AddRange(BitConverter.GetBytes(request.Id));

            var contractBytes = Encoding.ASCII.GetBytes(request.Contract);
            data.Add((byte)contractBytes.Length);
            data.AddRange(contractBytes);

            var operationBytes = Encoding.ASCII.GetBytes(request.Operation);
            data.Add((byte)operationBytes.Length);
            data.AddRange(operationBytes);

            if (request.HasParameters)
            {
                data.Add((byte)request.Parameters.Length);
                foreach (var p in request.Parameters)
                {
                    data.AddRange(BitConverter.GetBytes(p.Length));
                    data.AddRange(p);
                }
            }
            else
            {
                data.Add(0);
            }

            var dataSize = BitConverter.GetBytes(data.Count);

            var msg = new List<byte>();
            msg.AddRange(dataSize);
            msg.AddRange(data);

            var buffer = new ArraySegment<byte>(msg.ToArray());

            await this._Write(buffer);
        }

        private async Task<int> GetMessageSize()
        {
            var size = 4;
            var poolBuffer = this.BufferManager.GetFitBuffer(size);
            var buffer = new ArraySegment<byte>(poolBuffer, 0, size);
            await this.ReadBytes(buffer);
            var result = BitConverter.ToInt32(buffer.Array, 0);
            this.BufferManager.AddBuffer(poolBuffer);
            return result;
        }

        private async Task<ArraySegment<byte>> ReadBytes(ArraySegment<byte> buffer)
        {
            var read = 0;
            var length = buffer.Count;
            while (this.State == CommunicationState.Opened && this.Connected)
            {
                read += await this._Read(buffer);
                if (read == length)
                {
                    return buffer;
                }
            }
            throw new Exception("Stream is not readable");
        }

        public async Task<Message> WriteRequest(Message request, int timeout)
        {
            var responseEvent = new ResponseEvent();
            if (!this.mapper.TryAdd(request.Id, responseEvent))
            {
                this.Dispose();
                throw new Exception("Could not add request to the mapper");
            }
            await this.WriteMessage(request);
            return responseEvent.GetResponse(timeout);
        }

        private class ResponseEvent
        {
            private Message response;
            private bool IsSuccess { get; set; }
            private bool IsCompleted { get; set; }

            private readonly ManualResetEvent evt;

            public ResponseEvent()
            {
                this.evt = new ManualResetEvent(false);
            }

            public void SetResponse(Message response)
            {
                this.IsSuccess = true;
                this.response = response;
                this.evt.Set();
            }

            public Message GetResponse(int timeout)
            {
                this.evt.WaitOne(timeout);
                this.IsCompleted = true;

                if (this.IsSuccess == false)
                    throw new Exception("Receivetimeout reached without getting response");
                return response;
            }
        }
    }
}