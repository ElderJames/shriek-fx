using Shriek.ServiceProxy.Tcp.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp
{
    /// <summary>
    /// 表示SSL的Tcp会话对象
    /// </summary>
    internal class SslTcpSession : TcpSessionBase
    {
        /// <summary>
        /// 目标主机
        /// </summary>
        private string targetHost;

        /// <summary>
        /// SSL数据流
        /// </summary>
        private SslStream sslStream;

        /// <summary>
        /// 服务器证书
        /// 不为null时表示服务器会话对象
        /// </summary>
        private X509Certificate certificate;

        /// <summary>
        /// 远程证书验证回调
        /// </summary>
        private RemoteCertificateValidationCallback certificateValidationCallback;

        /// <summary>
        /// 缓冲区范围
        /// </summary>
        private readonly ArraySegment<byte> bufferRange = BufferPool.AllocBuffer();

        /// <summary>
        /// 获取会话是否提供SSL/TLS安全
        /// </summary>
        public override bool IsSecurity
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 表示SSL服务器会话对象
        /// </summary>
        /// <param name="certificate">服务器证书</param>
        ///  <exception cref="ArgumentNullException"></exception>
        public SslTcpSession(X509Certificate certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException();
            }
            this.certificate = certificate;
            this.certificateValidationCallback = (a, b, c, d) => true;
        }

        /// <summary>
        /// 表示SSL客户端会话对象
        /// </summary>
        /// <param name="targetHost">目标主机</param>
        /// <param name="certificateValidationCallback">远程证书验证回调</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SslTcpSession(string targetHost, RemoteCertificateValidationCallback certificateValidationCallback)
        {
            if (string.IsNullOrEmpty(targetHost) == true)
            {
                throw new ArgumentNullException("targetHost");
            }
            this.targetHost = targetHost;
            this.certificateValidationCallback = certificateValidationCallback;
        }

        /// <summary>
        /// 绑定一个Socket对象
        /// </summary>
        /// <param name="socket">套接字</param>
        public override void SetSocket(Socket socket)
        {
            var nsStream = new NetworkStream(socket, false);
            this.sslStream = new SslStream(nsStream, false, this.certificateValidationCallback);
            base.SetSocket(socket);
        }

        /// <summary>
        /// SSL验证
        /// </summary>
        /// <exception cref="System.Security.Authentication.AuthenticationException"></exception>
        public override void Authenticate()
        {
            // SSL客户端
            if (this.certificate == null)
            {
                this.sslStream.AuthenticateAsClient(this.targetHost);
            }
            else
            {
                this.sslStream.AuthenticateAsServer(this.certificate);
            }
        }

        /// <summary>
        /// 异步SSL验证
        /// </summary>
        /// <returns></returns>
        public override Task AuthenticateAsync()
        {
            // SSL客户端
            if (this.certificate == null)
            {
                return this.sslStream.AuthenticateAsClientAsync(this.targetHost);
            }
            else
            {
                return this.sslStream.AuthenticateAsServerAsync(this.certificate);
            }
        }

        /// <summary>
        /// 异步接收数据
        /// 将接收结果写入StreamReader
        /// 如果返回false表示接收异常
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> ReceiveAsync()
        {
            try
            {
                var read = await this.sslStream.ReadAsync(this.bufferRange.Array, this.bufferRange.Offset, this.bufferRange.Count);
                if (read <= 0)
                {
                    var handler = this.DisconnectHandler;
                    if (handler != null)
                    {
                        handler(this);
                    }
                    return false;
                }

                lock (this.StreamReader.SyncRoot)
                {
                    this.StreamReader.Stream.Seek(0, SeekOrigin.End);
                    this.StreamReader.Stream.Write(this.bufferRange.Array, this.bufferRange.Offset, read);
                    this.StreamReader.Stream.Seek(0, SeekOrigin.Begin);
                }

                var recvedHandler = this.ReceiveCompletedHandler;
                if (recvedHandler != null)
                {
                    await recvedHandler(this);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                var handler = this.DisconnectHandler;
                if (handler != null)
                {
                    handler(this);
                }
                return false;
            }
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="byteRange">数据范围</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public override int Send(ArraySegment<byte> byteRange)
        {
            if (byteRange == null)
            {
                throw new ArgumentNullException();
            }

            if (this.IsConnected == false)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }

            this.sslStream.Write(byteRange.Array, byteRange.Offset, byteRange.Count);
            return byteRange.Count;
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public override int Send(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            if (this.IsConnected == false)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }

            this.sslStream.Write(buffer);
            return buffer.Length;
        }

        /// <summary>
        /// 关闭会话的发送与接收
        /// </summary>
        /// <returns></returns>
        public override bool Shutdown()
        {
            var state = base.Shutdown();
            if (state == true)
            {
                this.sslStream.Dispose();
            }
            return state;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing == true)
            {
                this.targetHost = null;
                this.sslStream = null;
                this.certificate = null;
                this.certificateValidationCallback = null;
            }
        }
    }
}