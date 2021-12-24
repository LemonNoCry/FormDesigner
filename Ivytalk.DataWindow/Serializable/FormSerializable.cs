using System;
using System.Windows.Forms;
using Ivytalk.DataWindow.CustomPropertys;

namespace Ivytalk.DataWindow.Serializable
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