using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Ivytalk.DataWindow.Serializable.CustomizeProperty
{
    [Serializable]
    public class XmlType
    {
        Type type = typeof(Control);

        public XmlType()
        {
        }

        public XmlType(Type c)
        {
            if (c != null)
            {
                type = c;
            }
        }

        public static implicit operator Type(XmlType x)
        {
            if (x == null)
            {
                return typeof(Control);
            }

            return Type.GetType(x.TypeFullName);
        }

        public static implicit operator XmlType(Type c)
        {
            return new XmlType(c);
        }


        [XmlAttribute]
        public string TypeFullName
        {
            get => type + ", " + type.Assembly;
            set => type = Type.GetType(value);
        }
    }
}