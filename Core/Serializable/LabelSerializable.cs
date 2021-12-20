using System.Windows.Forms;

namespace FormDesinger.Core.Serializable
{
    public class LabelSerializable : ControlSerializable, IPropertyCollections<Label>
    {
        public bool AutoSize { get; set; }

        public CustomPropertyCollection GetCollections(Label control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("自动大小", "AutoSize", "布局", "标签自动调整大小", control));

            return cpc;
        }
    }
}