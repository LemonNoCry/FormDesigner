using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormDesinger.Core.Serializable
{
    ///<summary> 
    ///这里要添加对序列化的支持 
    ///</summary> 
    [Serializable]
    public class FormSerializable : ControlSerializable, IPropertyCollections<Form>
    {
        public double Opacity { get; set; }

        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as Form);
        }
        public CustomPropertyCollection GetCollections(Form control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("透明度", "Opacity", "外观", "Opacity 透明度", control));

            return cpc;
        }
    }
}