using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ivytalk.DataWindow.CustomPropertys;

namespace Ivytalk.DataWindow.Serializable
{
    public class LabelSerializable : ControlSerializable, IPropertyCollections<Label>
    {
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.TopLeft;

        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as Label);
        }

        public CustomPropertyCollection GetCollections(Label control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("对齐方式", "TextAlign", "外观", "对齐方式", control));
            return cpc;
        }
    }
}