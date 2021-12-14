

using System.IO;
using System.IO.Compression;


namespace Vorcyc.PowerLibrary.IO
{
    /// <summary>
    /// 使用.NET基类库中的压缩和解压缩
    /// </summary>
    public class Compression
    {

        /// <summary>
        /// 压缩文件到字节数组
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public static byte[] CompressFileToBytes(string file)
        {
            byte[] result;
            byte[] buffer = File.ReadAllBytes(file);

            //创建内存流对象，将数据压缩后放入内存流
            using (MemoryStream mems = new MemoryStream())
            {
                using (DeflateStream zipStream = new DeflateStream(mems, CompressionMode.Compress))
                {
                    // Write the contents of the buffer to the compressed stream.
                    zipStream.Write(buffer, 0, buffer.Length);

                    // Flush and close all streams.
                    zipStream.Flush();
                }
                result = mems.ToArray();

            }
            buffer = null;
            return result;

        }

        /// <summary>
        /// 解压缩字节数组到文件，首先要求传入的数组是已压缩的
        /// </summary>
        /// <param name="source">已压缩的字节数组</param>
        /// <param name="destFile">解压后的文件路径</param>
        public static void UncompressBytesToFile(byte[] source, string destFile)
        {
            using (FileStream OutStream = new FileStream(destFile, FileMode.Create))
            {
                using (MemoryStream mems = new MemoryStream(source))
                {
                    using (DeflateStream zipStream = new DeflateStream(mems, CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[source.Length];

                        //此时buffer中的数据已经是解压缩了的
                        zipStream.Read(buffer, 0, source.Length);

                        OutStream.Write(buffer, 0, source.Length);

                    }
                }
            }
        }


        ///// <summary>
        ///// 将一个文件的字节形式压缩至字节形式
        ///// </summary>
        ///// <param name="source">文件的字节数组</param>
        ///// <returns></returns>
        //public static byte[] CompressBytesToBytes(byte[] source)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        using (DeflateStream zipStream = new DeflateStream(memoryStream, CompressionMode.Compress))
        //        {
        //            zipStream.Write(source, 0, source.Length);
        //        }
        //        return memoryStream.ToArray();
        //    }
        //}


        ///// <summary>
        ///// 解压字节到字节，<see cref="CompressBytesToBytes(byte[])"/>的反函数
        ///// </summary>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //public static byte[] UncompressBytesToBytes(byte[] source)
        //{
        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        using (MemoryStream inStream = new MemoryStream(source))/*该流为过度流,因为:
        //            C#中，解压时不能像压缩时那样
        //            直接将解压缩后的流写到输出流，而需要是一个过度流
        //            VB中就没有这个现象，难道是C#的BUG？！
        //            但是第二天测试也出现同样问题*/
        //        {
        //            using (DeflateStream zipStream = new DeflateStream(inStream, CompressionMode.Decompress))
        //            {
        //                byte[] buffer = new byte[source.Length*2];
        //                zipStream.Read(buffer, 0, source.Length);

        //                outStream.Write(buffer, 0, source.Length);
        //            }
        //            return outStream.ToArray();
        //        }

        //    }
        //}


        public static byte[] CompressBytesToBytes(byte[] data)
        {
            using (var sourceStream = new MemoryStream(data))
            {
                using (var outStream = new MemoryStream())
                {
                    using (var compressionStream = new DeflateStream(outStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);

                        return outStream.ToArray();
                    }
                }
            }
        }

        public static byte[] DecompressBytesToBytes(byte[] data)
        {
            using (var sourceStream = new MemoryStream(data))
            {
                using (var outStream = new MemoryStream(1000))
                {
                    using (var compressionStream = new DeflateStream(outStream, CompressionMode.Decompress))
                    {
                        sourceStream.CopyTo(compressionStream);

                        return outStream.ToArray();
                    }
                }
            }
        }



    }
}
