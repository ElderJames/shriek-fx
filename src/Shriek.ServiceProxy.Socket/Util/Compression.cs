using System.IO;
using System.IO.Compression;

namespace Shriek.ServiceProxy.Socket.Util
{
    /// <summary>
    /// GZip
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Gzip压缩
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public static byte[] GZipCompress(byte[] buffer)
        {
            return Compression.GZipCompress(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Gzip压缩
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] GZipCompress(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return buffer;
            }

            using (var stream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    zipStream.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
            }
        }
    }
}