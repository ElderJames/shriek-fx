using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Abstractions.TcpClient
{
    public class TcpMessage
    {
        private readonly Encoding _encoder;
        private readonly byte _writeLineDelimiter;
        private readonly bool _autoTrim;

        public TcpMessage(byte[] data, System.Net.Sockets.TcpClient tcpClient, Encoding stringEncoder, byte lineDelimiter)
        {
            Data = data;
            TcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
        }

        public TcpMessage(byte[] data, System.Net.Sockets.TcpClient tcpClient, Encoding stringEncoder, byte lineDelimiter, bool autoTrim)
        {
            Data = data;
            TcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
            _autoTrim = autoTrim;
        }

        public byte[] Data { get; }

        public string MessageString => _autoTrim ? _encoder.GetString(Data).Trim() : _encoder.GetString(Data);

        public void Reply(byte[] data)
        {
            TcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void Reply(string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }
            Reply(_encoder.GetBytes(data));
        }

        public void ReplyLine(string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }
            if (data.LastOrDefault() != _writeLineDelimiter)
            {
                Reply(data + _encoder.GetString(new byte[] { _writeLineDelimiter }));
            }
            else
            {
                Reply(data);
            }
        }

        public System.Net.Sockets.TcpClient TcpClient { get; }
    }
}