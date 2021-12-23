using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormDesinger.Core.Serializable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ExpressionDebugger;
using Mapster;

namespace FormDesinger.Core.Serializable.Tests
{
    [TestClass()]
    public class FormSerializableTests
    {
        public static string SerializeXml(object data)
        {
            using (var sw = new StringWriter())
            {
                var xz = new XmlSerializer(data.GetType());
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

            frm = frm2.MapsterCopyTo(frm);
            Console.WriteLine(SerializeXml(frm));
        }

        [TestMethod]
        public void Method02()
        {
            Form frm2 = new Form()
            {
                Name = "asd",
                Text = "asdasd",
                Size = new Size(100, 100),
                BackColor = Color.Red,
                Location = new Point(10, 10),
            };

            ControlSerializable controlSerializable = new FormSerializable();
            frm2.MapsterCopyTo(controlSerializable);

            var xml = SerializeXml(controlSerializable);
            Console.WriteLine(xml);

            var ser = DeserializeXml<FormSerializable>(xml);
            Form frm3 = new Form();
            frm3 = ser.MapsterCopyTo(frm3);

            Console.WriteLine(frm3.Location);
            Console.WriteLine(1);
        }

        [TestMethod]
        public void Method03()
        {
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();

            Control lbl = new Label()
            {
                AutoSize = false
            };
            Control lbl2 = new Label()
            {
                AutoSize = true
            };
            LabelSerializable ls = lbl.MapsterCopyTo<LabelSerializable>();
           
            ControlSerializable ls2 = lbl2.MapsterCopyTo<LabelSerializable>();

            dynamic dyn = ls2;
            TypeAdapter.Adapt(dyn, ls);

            Console.WriteLine(ls.AutoSize);
        }
    }
}