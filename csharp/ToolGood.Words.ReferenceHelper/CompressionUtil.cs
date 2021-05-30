using System.IO;
using System.IO.Compression;
#if NETSTANDARD2_0
using Brotli;
#endif

namespace ToolGood.Bedrock
{
    /// <summary>
    /// 压缩
    /// </summary>
    public static class CompressionUtil
    {
        /// <summary>
        /// 默认压缩
        /// </summary>
        /// <param name="data">要压缩的字节数组</param>
        /// <param name="fastest">快速模式</param>
        /// <returns>压缩后的数组</returns>
        public static byte[] DeflateCompress(byte[] data, bool fastest = false)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream()) {
                    var level = fastest ? CompressionLevel.Fastest : CompressionLevel.Optimal;
                    using (DeflateStream zStream = new DeflateStream(stream, level)) {
                        zStream.Write(data, 0, data.Length);
                    }
                    return stream.ToArray();
                }
            } catch {
                return data;
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="data">要解压的字节数组</param>
        /// <returns>解压后的数组</returns>
        public static byte[] DeflateDecompression(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream(data)) {
                    using (DeflateStream zStream = new DeflateStream(stream, CompressionMode.Decompress)) {
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
                    var level = fastest ? CompressionLevel.Fastest : CompressionLevel.Optimal;
                    using (GZipStream zStream = new GZipStream(stream, level)) {
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


        /// <summary>
        /// Br压缩
        /// </summary>
        /// <param name="data">要压缩的字节数组</param>
        /// <param name="fastest">快速模式</param>
        /// <returns>压缩后的数组</returns>
        public static byte[] BrCompress(byte[] data, bool fastest = false)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream()) {
                    var level = fastest ? CompressionLevel.Fastest : CompressionLevel.Optimal;
                    using (BrotliStream zStream = new BrotliStream(stream, level)) {
                        zStream.Write(data, 0, data.Length);
                    }
                    return stream.ToArray();
                }
            } catch {
                return data;
            }
        }


        /// <summary>
        /// Br解压
        /// </summary>
        /// <param name="data">要解压的字节数组</param>
        /// <returns>解压后的数组</returns>
        public static byte[] BrDecompress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream(data)) {
                    using (BrotliStream zStream = new BrotliStream(stream, CompressionMode.Decompress)) {
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
