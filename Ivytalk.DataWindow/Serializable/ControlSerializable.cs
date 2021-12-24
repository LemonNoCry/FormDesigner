using Ivytalk.DataWindow.Serializable.CustomizeProperty;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ivytalk.DataWindow.CustomPropertys;

namespace Ivytalk.DataWindow.Serializable
{
    /// <summary>
    /// 控件序列化基类
    /// </summary>
    [Serializable]
    public class ControlSerializable : IPropertyCollections<Control>
    {
        public string Name { get; set; }
        public string Text { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        public Color BackColor { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        public Color ForeColor { get; set; }

        public CustomizeFont Font { get; set; }

        [XmlElement(Type = typeof(CustomizePoint))]
        public Point Location { get; set; }

        [XmlElement(Type = typeof(CustomizeSize))]
        public Size Size { get; set; }

        public bool AutoSize { get; set; }

        [XmlElement(Type = typeof(CustomizeRectangle))]
        public Rectangle ClientRectangle { get; set; }

        [XmlElement(Type = typeof(CustomizePadding))]
        public Padding Margin { get; set; }

        public AnchorStyles Anchor { get; set; }
        public DockStyle Dock { get; set; }
        public object Tag { get; set; }
        public int TabIndex { get; set; }

        public ControlSerializable ParentSerializable { get; set; }
        public List<ControlSerializable> ControlsSerializable { get; set; }

        public virtual CustomPropertyCollection GetCollections(Control control)
        {
            var collection = new CustomPropertyCollection();
            collection.Add(new CustomProperty("Name", "Name", "数据", "控件的Name", control) {IsReadOnly = true});
            collection.Add(new CustomProperty("文本", "Text", "数据", "Text 要显示的内容。", control, typeof(MultilineStringEditor)));
            collection.Add(new CustomProperty("背景色", "BackColor", "外观", "BackColor 背景色。", control));
            collection.Add(new CustomProperty("颜色", "ForeColor", "外观", "ForeColor 前景色。", control));
            collection.Add(new CustomProperty("字体", "Font", "外观", "Font 字体。", control));
            collection.Add(new CustomProperty("位置", "Location", "布局", "Location 控件左上角相对于其容器左上角的坐标。", control));
            collection.Add(new CustomProperty("大小", "Size", "布局", "Size 控件的大小，以像素为单位。", control));
            collection.Add(new CustomProperty("自动大小", "AutoSize", "布局", "自动调整大小以适应内容长度", control));
            collection.Add(new CustomProperty("边距", "Margin", "布局", "Margin 指定此控件与另一控件之间边距的距离。", control));
            collection.Add(new CustomProperty("锚", "Anchor", "布局", "Anchor 定义要绑定到容器的边缘，当控件锚定位到某个控件时，与指定边缘最接近的控件边缘与指定边缘之间的距离将保持不变。", control));
            collection.Add(new CustomProperty("停靠", "Dock", "布局", "Dock 定义要绑定到容器的控件边框。", control));
            collection.Add(new CustomProperty("Tab索引", "TabIndex", "行为", "TabIndex 确定此控件将占用的 Tab 键顺序索引。", control));

            control.Tag = control.Tag ?? "";
            collection.Add(new CustomProperty("Tag", "Tag", "行为", "与用户关联的自定义数据", control));
            return collection;
        }
    }
}