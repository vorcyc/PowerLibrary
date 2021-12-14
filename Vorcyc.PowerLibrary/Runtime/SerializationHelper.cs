using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Vorcyc.PowerLibrary.Runtime
{
    /// <summary>
    ///持久化工具
    /// </summary>
    public static class SerializationHelper
    {

        private static object _lock = new object();


        /// <summary>
        /// 持久化一个对象实例到文件
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="instance"></param>
        /// <param name="filePath"></param>
        public static void Serialize(SerializeMode mode, object instance, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                MemoryStream stream = Serialize(mode, instance);
                byte[] buffer = stream.ToArray();
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 序列化任何可序列对象
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static MemoryStream Serialize(SerializeMode mode, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("实例对象为空.");

            lock (_lock)
            {
                MemoryStream stream = new MemoryStream();
                switch (mode)
                {
                    case SerializeMode.Xml:
                        SerializeXml(ref stream, ref instance);
                        return stream;

                    default:
                        SerializeBinary(ref stream, ref instance);
                        return stream;
                }
            }
        }

        private static void SerializeBinary(ref MemoryStream stream, ref object instance)
        {
            BinaryFormatter bfor = new BinaryFormatter();
            bfor.Serialize(stream, instance);
            bfor = null;
        }

        private static void SerializeXml(ref MemoryStream stream, ref object instance)
        {
            XmlSerializer ser = new XmlSerializer(instance.GetType());
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, null);
            ser.Serialize(stream, instance, xsn);
            ser = null;
        }


        /// <summary>
        /// 从数据流反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <param name="instance"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T Deserialize<T>(SerializeMode mode, T instance, byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("字节流为空.");

            lock (_lock)
            {
                switch (mode)
                {
                    case SerializeMode.Xml:
                        return DeserializeXml<T>(instance, buffer);

                    default:
                        return DeserializeBinary<T>(instance, buffer);
                }
            }
        }

        /// <summary>
        /// 从文件反持久化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <param name="instance"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(SerializeMode mode, T instance, string filePath)
        {
            byte[] buffer = null;

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    return Deserialize<T>(mode, instance, buffer);
                }
            }
            finally
            {
                buffer = null;
            }
        }

        /// <summary>
        /// 从字节流以XML反序列化
        /// </summary>
        private static T DeserializeXml<T>(T instance, byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                instance = (T)ser.Deserialize(stream);
                return instance;
            }
        }


        /// <summary>
        /// 从字节流二进制反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        private static T DeserializeBinary<T>(T instance, byte[] array)
        {
            BinaryFormatter bf;
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream(array);
                {
                    bf = new BinaryFormatter();
                    instance = (T)bf.Deserialize(ms);
                    return instance;
                }
            }
            finally
            {
                bf = null;
                if (ms != null) ms.Close();
                ms = null;
            }
        }

    }


    /// <summary>
    /// 序列化模式
    /// </summary>
    public enum SerializeMode
    {
        Binary,
        Xml
    }
 

}
