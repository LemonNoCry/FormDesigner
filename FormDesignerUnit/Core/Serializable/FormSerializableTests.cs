using Ivytalk.DataWindow.Serializable;
using Ivytalk.DataWindow.Utility;
using Mapster;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ivytalk.DataWindow.Core;
using Ivytalk.DataWindow.DesignLayer;

namespace FormDesinger.Core.Serializable.Tests
{
    [TestClass()]
    public class FormSerializableTests
    {
        public static string SerializeXml(ControlSerializable data)
        {
            using (var sw = new StringWriter())
            {
                var xz = Collections.GetXmlSerializer();
                xz.Serialize(sw, data);
                return sw.ToString();
            }
        }

        public static T DeserializeXml<T>(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xz = new XmlSerializer(typeof(T));
                T dept = (T) xz.Deserialize(sr);
                return dept;
            }
        }

        [TestMethod()]
        public void GetObjectDataTest()
        {
            FormSerializable frm = new FormSerializable();

            //为了方便测试定义内存流
            MemoryStream ms = new MemoryStream();
            BinaryFormatter form = new BinaryFormatter();

            //对对象进行序列化
            form.Serialize(ms, frm);
            ms.Flush();
            //获取流中的数据以便反序列化
            byte[] bts = ms.GetBuffer();

            Console.WriteLine(Encoding.UTF8.GetString(bts));
        }

        [TestMethod]
        public void Method01()
        {
            Form frm2 = new Form()
            {
                Text = "asdasd",
                Size = new Size(100, 100)
            };
            FormSerializable frm = new FormSerializable();
            frm.Text = "asd";

            frm2.MapsterCopyTo(frm);
            Console.WriteLine(SerializeXml(frm));
        }

        [TestMethod]
        public void Method02()
        {
            Form frm = new Form();
            BaseDataWindow frm2 = new BaseDataWindow()
            {
                Name = "asd",
                Text = "asdasd",
                Size = new Size(100, 100),
                BackColor = Color.Red,
                Location = new Point(10, 10),
                Font = new Font("宋体", 12, FontStyle.Bold)
            };
            frm2.Controls.Add(new Label());
            frm2.Controls.Add(new TextBox());

            frm2.TopLevel = false;
            frm.Controls.Add(frm2);

            ControlSerializable controlSerializable = DataWindowAnalysis.GetSerializationControls(frm2);
            var xml = DataWindowAnalysis.SerializationControls(frm2);

            Console.WriteLine(xml);

            ControlSerializable cs = DataWindowAnalysis.DeserializeControls(xml);

            Console.WriteLine(cs.GetType());
            //var ser = DeserializeXml<FormSerializable>(xml);
            //Form frm3 = new Form();
            //frm3 = ser.MapsterCopyTo(frm3);

            //Console.WriteLine(frm3.Location);
            //Console.WriteLine(1);
        }

        [TestMethod]
        public void Method0111()
        {
            Console.WriteLine(new LabelSerializable().TextAlign);
        }
        [TestMethod]
        public void Method011()
        {
            Console.WriteLine(Type.GetType("System.Windows.Forms.Form, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
            Console.WriteLine(typeof(Form).Assembly);
        }

        [TestMethod]
        public void Method03()
        {
            System.Drawing.Font font1 = new Font("宋体", 9);
            Byte[] bytes = Serializable.SerializeObject(font1);
            System.Drawing.Font font2 = Serializable.DeserializeObject(bytes) as System.Drawing.Font;

            Console.WriteLine(font2);
        }

        [TestMethod]
        public void Method04()
        {
            Form frm2 = new Form()
            {
                Name = "asd",
                Text = "asdasd",
                Size = new Size(100, 100),
                BackColor = Color.Red,
                Location = new Point(10, 10),
            };

            Byte[] bytes = Serializable.SerializeObject(frm2);

            var frm = Serializable.DeserializeObject(bytes) as Form;

            Console.WriteLine(frm.Name);
        }

        /// <summary>
        /// 对象序列化对象类
        /// </summary>
        public class Serializable
        {
            private Serializable()
            {
                //
                // TODO: 在此处添加构造函数逻辑
                //
            }

            /// <summary>
            /// 把对象序列化并返回相应的字节
            /// </summary>
            /// <param name="pObj">需要序列化的对象</param>
            /// <returns>byte[]</returns>
            public static byte[] SerializeObject(object pObj)
            {
                if (pObj == null)
                    return null;
                System.IO.MemoryStream _memory = new System.IO.MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(_memory, pObj);
                _memory.Position = 0;
                byte[] read = new byte[_memory.Length];
                _memory.Read(read, 0, read.Length);
                _memory.Close();
                return read;
            }

            public static void SerializeObject(object pObj, string pFileName)
            {
                System.IO.FileStream stream = System.IO.File.Open(pFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                byte[] bytes = SerializeObject(pObj);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }

            public static object DeserializeObject(string pFileName)
            {
                System.IO.FileStream stream = System.IO.File.Open(pFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Close();
                return DeserializeObject(bytes);
            }

            /// <summary>
            /// 把字节反序列化成相应的对象
            /// </summary>
            /// <param name="pBytes">字节流</param>
            /// <returns>object</returns>
            public static object DeserializeObject(byte[] pBytes)
            {
                object _newOjb = null;
                if (pBytes == null)
                    return _newOjb;
                System.IO.MemoryStream _memory = new System.IO.MemoryStream(pBytes);
                _memory.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                _newOjb = formatter.Deserialize(_memory);
                _memory.Close();
                return _newOjb;
            }
        }
    }
}