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

            return x.type;
        }

        public static implicit operator XmlType(Type c)
        {
            return new XmlType(c);
        }

        [XmlAttribute]
        public string TypeName
        {
            get => type.FullName;
            set
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var ass in assemblies)
                {
                    type = ass.GetType(value);
                    if (type != null)
                    {
                        return;
                    }
                }

                if (type == null)
                {
                    type = typeof(Control);
                }
            }
        }
    }
}