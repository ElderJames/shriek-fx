using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Shriek.ServiceProxy.Socket.Util
{
    /// <summary>
    /// 提供获取Tcp端口快照信息
    /// </summary>
    public static class TcpSnapshot
    {
        /// <summary>
        /// 表示端口的占用进程的id
        /// </summary>
        [DebuggerDisplay("Port = {Port}, OwerPid = {OwerPid}")]
        public class PortOwnerPid
        {
            /// <summary>
            /// 获取Tcp端口
            /// </summary>
            public int Port { get; internal set; }

            /// <summary>
            /// 获取占用端口的进程id
            /// </summary>
            public int OwerPid { get; internal set; }

            /// <summary>
            /// 杀掉占用端口的进程
            /// </summary>
            /// <returns></returns>
            public bool Kill()
            {
                try
                {
                    var process = Process.GetProcessById(this.OwerPid);
                    if (process != null)
                    {
                        process.Kill();
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.Port.GetHashCode() ^ this.OwerPid.GetHashCode();
            }

            /// <summary>
            /// 比较是否相等
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return obj != null && this.GetHashCode() == obj.GetHashCode();
            }
        }

        /// <summary>
        /// 端口进程信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint State;
            public uint LocalAddr;
            public uint LocalPort;
            public uint RemoteAddr;
            public byte RemotePort;
            public uint OwningPid;
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private unsafe static extern uint GetExtendedTcpTable(
            byte* pTcpTable,
            int* dwOutBufLen,
            bool sort,
            AddressFamily ipVersion,
            int tblEnum,
            int reserved);

        /// <summary>
        /// 获取一次IPv4的Tcp端口快照信息
        /// </summary>
        /// <returns></returns>
        public unsafe static PortOwnerPid[] Snapshot()
        {
            return TcpSnapshot.Snapshot(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// 获取一次Tcp端口快照信息
        /// </summary>
        /// <param name="ipVersion">ip版本</param>
        /// <returns></returns>
        public unsafe static PortOwnerPid[] Snapshot(AddressFamily ipVersion)
        {
            var size = 0;
            var TCP_TABLE_OWNER_PID_ALL = 5;
            TcpSnapshot.GetExtendedTcpTable(null, &size, false, ipVersion, TCP_TABLE_OWNER_PID_ALL, 0);

            var pTable = stackalloc byte[size];
            var state = TcpSnapshot.GetExtendedTcpTable(pTable, &size, false, ipVersion, TCP_TABLE_OWNER_PID_ALL, 0);
            if (state != 0)
            {
                return new PortOwnerPid[0];
            }

            var hashSet = new HashSet<PortOwnerPid>();
            var rowLength = *(int*)(pTable);
            var pRow = pTable + Marshal.SizeOf(rowLength);

            for (int i = 0; i < rowLength; i++)
            {
                var row = *(MIB_TCPROW_OWNER_PID*)pRow;
                var port = ByteConverter.ToBytes(row.LocalPort, Endians.Little);

                var portOwner = new PortOwnerPid
                {
                    Port = ByteConverter.ToUInt16(port, 0, Endians.Big),
                    OwerPid = (int)row.OwningPid
                };
                hashSet.Add(portOwner);
                pRow = pRow + Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID));
            }

            return hashSet.OrderBy(item => item.Port).ToArray();
        }
    }
}