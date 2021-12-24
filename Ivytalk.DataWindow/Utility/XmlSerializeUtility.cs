using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ivytalk.DataWindow.Utility
{
    public static class XmlSerializeUtility
    {
        private static void XmlSerializeInternal<T>(Stream stream, T o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize<T>(this T obj, Encoding encoding)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            using (var stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, obj, encoding);

                stream.Position = 0;
                using (var reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile<T>(this T obj, string path, Encoding encoding)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializeInternal(file, obj, encoding);
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="xml">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string xml, Encoding encoding)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException(nameof(xml));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(xml)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T) mySerializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding);
        }
    }
}