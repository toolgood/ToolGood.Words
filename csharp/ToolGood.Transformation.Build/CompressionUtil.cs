using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace ToolGood.Transformation.Build
{
    /// <summary>
    /// 压缩
    /// </summary>
    public static class CompressionUtil
    {
 
        /// <summary>
        /// Gzip压缩
        /// </summary>
        /// <param name="data">要压缩的字节数组</param>
        /// <param name="fastest">快速模式</param>
        /// <returns>压缩后的数组</returns>
        public static byte[] GzipCompress(byte[] data, bool fastest = false)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream()) {
                     using (GZipStream zStream = new GZipStream(stream,  CompressionMode.Compress)) {
                        zStream.Write(data, 0, data.Length);
                    }
                    return stream.ToArray();
                }
            } catch {
                return data;
            }
        }

        /// <summary>
        /// Gzip解压
        /// </summary>
        /// <param name="data">要解压的字节数组</param>
        /// <returns>解压后的数组</returns>
        public static byte[] GzipDecompress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream(data)) {
                    using (GZipStream zStream = new GZipStream(stream, CompressionMode.Decompress)) {
                        using (var resultStream = new MemoryStream()) {
                            zStream.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    }
                }
            } catch {
                return data;
            }
        }
 
    }
}
