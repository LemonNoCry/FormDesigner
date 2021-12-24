using System;
using System.Windows.Forms;
using Ivytalk.DataWindow.CustomPropertys;
using Ivytalk.DataWindow.DesignLayer;

namespace Ivytalk.DataWindow.Serializable
{
    [Serializable]
    public class BaseDataWindowSerializable : FormSerializable, IPropertyCollections<BaseDataWindow>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as BaseDataWindow);
        }

        public CustomPropertyCollection GetCollections(BaseDataWindow control)
        {
            var cpc = base.GetCollections(control);
            cpc.Find(x => x.Name == "背景色").IsReadOnly = true;
            cpc.Find(x => x.Name == "位置").IsReadOnly = true;
            cpc.Find(x => x.Name == "边距").IsReadOnly = true;
            cpc.Find(x => x.Name == "锚").IsReadOnly = true;
            cpc.Find(x => x.Name == "停靠").IsReadOnly = true;
            cpc.Find(x => x.Name == "Tab索引").IsReadOnly = true;
            return cpc;
        }
    }
}