using Shriek.ServiceProxy.Abstractions.TcpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Shriek.ServiceProxy.Tcp.Server
{
    public class ShriekTcpServer
    {
        public ShriekTcpServer()
        {
            Delimiter = 0x13;
            StringEncoder = Encoding.UTF8;
        }

        private readonly List<Server.ServerListener> _listeners = new List<Server.ServerListener>();
        public byte Delimiter { get; set; }
        public System.Text.Encoding StringEncoder { get; set; }
        public bool AutoTrimStrings { get; set; }

        public event EventHandler<TcpClient> ClientConnected;

        public event EventHandler<TcpClient> ClientDisconnected;

        public event EventHandler<TcpMessage> DelimiterDataReceived;

        public event EventHandler<TcpMessage> DataReceived;

        public IEnumerable<IPAddress> GetIpAddresses()
        {
            var ipAddresses = new List<IPAddress>();

            var enabledNetInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up);

            foreach (var netInterface in enabledNetInterfaces)
            {
                var ipProps = netInterface.GetIPProperties();
                foreach (var addr in ipProps.UnicastAddresses)
                {
                    if (!ipAddresses.Contains(addr.Address))
                    {
                        ipAddresses.Add(addr.Address);
                    }
                }
            }

            var ipSorted = ipAddresses.OrderByDescending(RankIpAddress).ToList();
            return ipSorted;
        }

        public List<IPAddress> GetListeningIPs()
        {
            var listenIps = new List<IPAddress>();
            foreach (var l in _listeners)
            {
                if (!listenIps.Contains(l.IPAddress))
                {
                    listenIps.Add(l.IPAddress);
                }
            }

            return listenIps.OrderByDescending(RankIpAddress).ToList();
        }

        public void Broadcast(byte[] data)
        {
            foreach (var client in _listeners.SelectMany(x => x.ConnectedClients))
            {
                client.GetStream().Write(data, 0, data.Length);
            }
        }

        public void Broadcast(string data)
        {
            if (data == null) { return; }
            Broadcast(StringEncoder.GetBytes(data));
        }

        public void BroadcastLine(string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }
            if (data.LastOrDefault() != Delimiter)
            {
                Broadcast(data + StringEncoder.GetString(new[] { Delimiter }));
            }
            else
            {
                Broadcast(data);
            }
        }

        private static int RankIpAddress(IPAddress addr)
        {
            var rankScore = 1000;

            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                rankScore += 100;
            }

            // class A
            if (addr.ToString().StartsWith("10."))
            {
                rankScore += 100;
            }

            // class B
            if (addr.ToString().StartsWith("172.30."))
            {
                rankScore += 100;
            }

            // class C
            if (addr.ToString().StartsWith("192.168.1."))
            {
                rankScore += 100;
            }

            // local sucks
            if (addr.ToString().StartsWith("169."))
            {
                rankScore = 0;
            }

            return rankScore;
        }

        public ShriekTcpServer Start(int port, bool ignoreNicsWithOccupiedPorts = true)
        {
            var ipSorted = GetIpAddresses();
            var anyNicFailed = false;
            foreach (var ipAddr in ipSorted)
            {
                try
                {
                    Start(ipAddr, port);
                }
                catch (SocketException ex)
                {
                    DebugInfo(ex.ToString());
                    anyNicFailed = true;
                }
            }

            if (!IsStarted)
                throw new InvalidOperationException("Port was already occupied for all network interfaces");

            if (!anyNicFailed || ignoreNicsWithOccupiedPorts) return this;
            Stop();
            throw new InvalidOperationException("Port was already occupied for one or more network interfaces.");
        }

        public ShriekTcpServer Start(int port, AddressFamily addressFamilyFilter)
        {
            var ipSorted = GetIpAddresses().Where(ip => ip.AddressFamily == addressFamilyFilter);
            foreach (var ipAddr in ipSorted)
            {
                try
                {
                    Start(ipAddr, port);
                }
                catch
                {
                    // ignored
                }
            }

            return this;
        }

        public bool IsStarted => _listeners.Any(l => l.Listener.Active);

        public ShriekTcpServer Start(IPAddress ipAddress, int port)
        {
            ServerListener listener = new ServerListener(this, ipAddress, port);
            _listeners.Add(listener);

            return this;
        }

        public void Stop()
        {
            var all = _listeners.All(l => l.QueueStop = true);
            while (_listeners.Any(l => l.Listener.Active))
            {
                Thread.Sleep(100);
            }
            _listeners.Clear();
        }

        public int ConnectedClientsCount => _listeners.Sum(l => l.ConnectedClientsCount);

        internal void NotifyDelimiterMessageRx(ServerListener listener, TcpClient client, byte[] msg)
        {
            if (DelimiterDataReceived == null) return;
            var m = new TcpMessage(msg, client, StringEncoder, Delimiter, AutoTrimStrings);
            DelimiterDataReceived(this, m);
        }

        internal void NotifyEndTransmissionRx(ServerListener listener, TcpClient client, byte[] msg)
        {
            if (DataReceived == null) return;
            var m = new TcpMessage(msg, client, StringEncoder, Delimiter, AutoTrimStrings);
            DataReceived(this, m);
        }

        internal void NotifyClientConnected(ServerListener listener, TcpClient newClient)
        {
            ClientConnected?.Invoke(this, newClient);
        }

        internal void NotifyClientDisconnected(ServerListener listener, TcpClient disconnectedClient)
        {
            ClientDisconnected?.Invoke(this, disconnectedClient);
        }

        #region Debug logging

        [System.Diagnostics.Conditional("DEBUG")]
        private void DebugInfo(string format, params object[] args)
        {
            if (_debugInfoTime == null)
            {
                _debugInfoTime = new System.Diagnostics.Stopwatch();
                _debugInfoTime.Start();
            }
            System.Diagnostics.Debug.WriteLine(_debugInfoTime.ElapsedMilliseconds + ": " + format, args);
        }

        private System.Diagnostics.Stopwatch _debugInfoTime;

        #endregion Debug logging
    }
}