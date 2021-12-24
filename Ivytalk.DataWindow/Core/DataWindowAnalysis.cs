using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Ivytalk.DataWindow.Serializable;
using Ivytalk.DataWindow.Utility;

namespace Ivytalk.DataWindow.Core
{
    public class DataWindowAnalysis
    {
        public static Encoding Encoding = Encoding.UTF8;

        public static ControlSerializable GetSerializationControls(Control control)
        {
            var controlSerializable = Collections.ControlConvertSerializable(control);
            if (control.HasChildren)
            {
                controlSerializable.ControlsSerializable = new List<ControlSerializable>();
                foreach (Control con in control.Controls)
                {
                    var cs = GetSerializationControls(con);
                    controlSerializable.ControlsSerializable.Add(cs);
                }
            }

            return controlSerializable;
        }

        #region 序列化

        public static string SerializationControls(Control control)
        {
            return GetSerializationControls(control).XmlSerialize(Encoding);
        }

        public static string SerializationControls(ControlSerializable controlSerializable)
        {
            return controlSerializable.XmlSerialize(Encoding);
        }

        public static void SerializationControls(Control control, string path)
        {
            GetSerializationControls(control).XmlSerializeToFile(path, Encoding);
        }

        public static ControlSerializable DeserializeControls(string xml)
        {
            return XmlSerializeUtility.XmlDeserialize<ControlSerializable>(xml, Encoding);
        }

        public static ControlSerializable DeserializeControlsForPath(string path)
        {
            return XmlSerializeUtility.XmlDeserializeFromFile<ControlSerializable>(path, Encoding);
        }

        #endregion


    }
}