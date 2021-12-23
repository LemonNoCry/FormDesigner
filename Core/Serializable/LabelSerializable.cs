using System.Windows.Forms;

namespace FormDesinger.Core.Serializable
{
    public class LabelSerializable : ControlSerializable, IPropertyCollections<Label>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as Label);
        }

        public CustomPropertyCollection GetCollections(Label control)
        {
            var cpc = base.GetCollections(control);
            return cpc;
        }
    }
}