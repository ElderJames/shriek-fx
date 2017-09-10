using Shriek.ServiceProxy.Abstractions.TcpClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Shriek.ServiceProxy.Tcp
{
    public class ShriekTcpClient : IDisposable
    {
        public ShriekTcpClient()
        {
            StringEncoder = System.Text.Encoding.UTF8;
            ReadLoopIntervalMs = 10;
            Delimiter = 0x13;
        }

        private Thread _rxThread;
        private readonly List<byte> _queuedMsg = new List<byte>();
        public byte Delimiter { get; set; }
        public System.Text.Encoding StringEncoder { get; set; }

        public event EventHandler<TcpMessage> DelimiterDataReceived;

        public event EventHandler<TcpMessage> DataReceived;

        internal bool QueueStop { get; set; }
        internal int ReadLoopIntervalMs { get; set; }
        public bool AutoTrimStrings { get; set; }

        public ShriekTcpClient Connect(string hostNameOrIpAddress, int port)
        {
            if (string.IsNullOrEmpty(hostNameOrIpAddress))
            {
                throw new ArgumentNullException(nameof(hostNameOrIpAddress));
            }

            _TcpClient = new System.Net.Sockets.TcpClient();
            _TcpClient.Connect(hostNameOrIpAddress, port);

            StartRxThread();

            return this;
        }

        private void StartRxThread()
        {
            if (_rxThread != null) { return; }

            _rxThread = new Thread(ListenerLoop) { IsBackground = true };
            _rxThread.Start();
        }

        public ShriekTcpClient Disconnect()
        {
            if (_TcpClient == null) { return this; }
            _TcpClient.Close();
            _TcpClient = null;
            return this;
        }

        public System.Net.Sockets.TcpClient _TcpClient { get; private set; }

        private void ListenerLoop(object state)
        {
            while (!QueueStop)
            {
                try
                {
                    RunLoopStep();
                }
                catch
                {
                    // ignored
                }

                Thread.Sleep(ReadLoopIntervalMs);
            }

            _rxThread = null;
        }

        private void RunLoopStep()
        {
            if (_TcpClient == null) { return; }
            if (_TcpClient.Connected == false) { return; }

            var delimiter = this.Delimiter;
            var c = _TcpClient;

            var bytesAvailable = c.Available;
            if (bytesAvailable == 0)
            {
                Thread.Sleep(10);
                return;
            }

            var bytesReceived = new List<byte>();

            while (c.Available > 0 && c.Connected)
            {
                var nextByte = new byte[1];
                c.Client.Receive(nextByte, 0, 1, SocketFlags.None);
                bytesReceived.AddRange(nextByte);
                if (nextByte[0] == delimiter)
                {
                    var msg = _queuedMsg.ToArray();
                    _queuedMsg.Clear();
                    NotifyDelimiterMessageRx(c, msg);
                }
                else
                {
                    _queuedMsg.AddRange(nextByte);
                }
            }

            if (bytesReceived.Count > 0)
            {
                NotifyEndTransmissionRx(c, bytesReceived.ToArray());
            }
        }

        private void NotifyDelimiterMessageRx(System.Net.Sockets.TcpClient client, byte[] msg)
        {
            if (DelimiterDataReceived == null) return;
            var m = new TcpMessage(msg, client, StringEncoder, Delimiter, AutoTrimStrings);
            DelimiterDataReceived(this, m);
        }

        private void NotifyEndTransmissionRx(System.Net.Sockets.TcpClient client, byte[] msg)
        {
            if (DataReceived == null) return;
            var m = new TcpMessage(msg, client, StringEncoder, Delimiter, AutoTrimStrings);
            DataReceived(this, m);
        }

        public void Write(byte[] data)
        {
            if (_TcpClient == null) { throw new Exception("Cannot send data to a null TcpClient (check to see if Connect was called)"); }
            _TcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void Write(string data)
        {
            if (data == null) { return; }
            Write(StringEncoder.GetBytes(data));
        }

        public void WriteLine(string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }
            if (data.LastOrDefault() != Delimiter)
            {
                Write(data + StringEncoder.GetString(new[] { Delimiter }));
            }
            else
            {
                Write(data);
            }
        }

        public TcpMessage WriteLineAndGetReply(string data, TimeSpan timeout)
        {
            TcpMessage mReply = null;
            DataReceived += (s, e) => { mReply = e; };
            WriteLine(data);

            var sw = new Stopwatch();
            sw.Start();

            while (mReply == null && sw.Elapsed < timeout)
            {
                Thread.Sleep(10);
            }

            return mReply;
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
            }

            QueueStop = true;
            if (_TcpClient != null)
            {
                try
                {
                    _TcpClient.Close();
                }
                catch
                {
                    // ignored
                }
                _TcpClient = null;
            }

            _disposedValue = true;
        }

        ~ShriekTcpClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}