using System.Windows.Forms;

namespace FormDesinger.Core.Serializable
{
    public class CusFormSerializable : FormSerializable, IPropertyCollections<HostFrame>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as HostFrame);
        }

        public CustomPropertyCollection GetCollections(HostFrame control)
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